using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LeagueSquadApi.Services
{
    public class SquadService : ISquadService
    {
        private readonly AppDbContext db;
        public SquadService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<ServiceResult<SquadResponse>> GetAsync(long id, CancellationToken ct)
        {
            var squad = await db.Squad.Where(s => s.Id == id).FirstOrDefaultAsync(ct);
            if (squad == null) return ServiceResult<SquadResponse>.Fail(ResultStatus.NotFound);
            return ServiceResult<SquadResponse>.Ok(new SquadResponse(squad.Id, squad.Name, squad.IconUrl, squad.CreatedAt));
        }

        public async Task<ServiceResult<List<SquadResponse>>> GetAllAsync(int userId, CancellationToken ct)
        {
            // add user id check
            var squads = await db.Squad.Where(u => u.CreatorId == userId).OrderBy(s => s.Id).Select(s => new SquadResponse(s.Id, s.Name, s.IconUrl, s.CreatedAt)).ToListAsync(ct);
            return ServiceResult<List<SquadResponse>>.Ok(squads);
        }

        public async Task<ServiceResult<SquadResponse>> AddAsync(int userId, SquadRequest req, CancellationToken ct)
        {
            Squad squad = new Squad() { Name = req.Name, CreatorId = userId, IconUrl = req.IconUrl };
            await db.Squad.AddAsync(squad, ct);
            await db.SaveChangesAsync(ct);
            return ServiceResult<SquadResponse>.Ok(new SquadResponse(squad.Id, squad.Name, squad.IconUrl, squad.CreatedAt), ResultStatus.Created);
        }

        public async Task<ServiceResult<SquadResponse>> UpdateAsync(long id, SquadRequest req, CancellationToken ct)
        {
            var squad = await db.Squad.Where(s => s.Id == id).FirstOrDefaultAsync(ct);
            if (squad == null) return ServiceResult<SquadResponse>.Fail(ResultStatus.NotFound);
            squad.Name = req.Name;
            squad.IconUrl = req.IconUrl;
            await db.SaveChangesAsync(ct);
            return ServiceResult<SquadResponse>.Ok(new SquadResponse(squad.Id, squad.Name, squad.IconUrl, squad.CreatedAt));
        }

        public async Task<ServiceResult> DeleteAsync(long id, CancellationToken ct)
        {
            var numDeleted = await db.Squad.Where(s => s.Id == id).ExecuteDeleteAsync(ct);
            if (numDeleted == 0) return ServiceResult.Fail(ResultStatus.NotFound);
            return ServiceResult.Ok(ResultStatus.NoContent);
        }

        public async Task<ServiceResult<List<SquadMemberResponse>>> GetAllMembersAsync(long id, CancellationToken ct)
        {
            var squad = await db.Squad.FindAsync(id, ct);
            if (squad == null) return ServiceResult<List<SquadMemberResponse>>.Fail(ResultStatus.NotFound);
            var squadMembers = await db.SquadMember.Where(sm => sm.SquadId == id).Join(db.Player, sm => sm.Puuid, p => p.Id, (sm, p) =>
                new SquadMemberResponse(sm.SquadId, sm.Puuid, sm.Role ?? "", sm.Alias ?? "", sm.CreatedAt, p.GameName, p.TagLine, p.Region ?? "")).ToListAsync(ct);
            return ServiceResult<List<SquadMemberResponse>>.Ok(squadMembers);
        }

        public async Task<ServiceResult<SquadMemberResponse>> AddMemberAsync(long id, SquadMemberRequest req, IPlayerService ps, CancellationToken ct)
        {
            var existingSquadMembers = await db.SquadMember.Where(sm => sm.SquadId == id).ToListAsync(ct);
            var memberCount = existingSquadMembers.Count();
            if (memberCount == 5) return ServiceResult<SquadMemberResponse>.Fail(ResultStatus.Conflict);

            var exists = await db.SquadMember.Where(sm => sm.SquadId == id && sm.Puuid == req.Puuid).FirstOrDefaultAsync(ct);
            if (exists != null) return ServiceResult<SquadMemberResponse>.Fail(ResultStatus.Conflict);

            var res = await ps.UpsertWithPuuidAsync(req.Puuid, ct);
            if (!res.IsSuccessful) return ServiceResult<SquadMemberResponse>.Fail(res.Status);
            var p = res.Value;
            if (p == null) return ServiceResult<SquadMemberResponse>.Fail(ResultStatus.Unknown);
            SquadMember sm = new SquadMember() { SquadId = id, Puuid = req.Puuid, Role = req.Role, Alias = req.Alias };
            await db.SquadMember.AddAsync(sm, ct);
            await db.SaveChangesAsync(ct);
            return ServiceResult<SquadMemberResponse>.Ok(new SquadMemberResponse(sm.SquadId, sm.Puuid, sm.Role, sm.Alias, p.CreatedAt, p.GameName, p.TagLine, p.Region), ResultStatus.Created);
        }

        public async Task<ServiceResult<SquadMemberResponse>> GetMemberAsync(long id, string puuid, CancellationToken ct)
        {
            var sm = await db.SquadMember.Where(sm => sm.Puuid == puuid && sm.SquadId == id).Join(db.Player, sm => sm.Puuid, p => p.Id, (sm, p) =>
                new SquadMemberResponse(sm.SquadId, sm.Puuid, sm.Role ?? "", sm.Alias ?? "", sm.CreatedAt, p.GameName, p.TagLine, p.Region ?? "")).FirstOrDefaultAsync(ct);
            if (sm == null) return ServiceResult<SquadMemberResponse>.Fail(ResultStatus.NotFound);
            return ServiceResult<SquadMemberResponse>.Ok(sm);
        }

        public async Task<ServiceResult> DeleteMemberAsync(long id, string puuid, CancellationToken ct)
        {
            var numDeleted = await db.SquadMember.Where(sm => sm.SquadId == id && sm.Puuid == puuid).ExecuteDeleteAsync(ct);
            if (numDeleted == 0) return ServiceResult.Fail(ResultStatus.NotFound);
            return ServiceResult.Ok(ResultStatus.NoContent);
        }

        public async Task<ServiceResult<List<SquadMatchResponse>>> GetSquadMatchesAsync(long id, IRiotService rs, IMatchService ms, ISquadMatchService sms, CancellationToken ct, bool forceRefresh = false)
        {
            // final return value
            var squadMatches = new List<SquadMatchResponse>();


            // get squad based on id
            var squad = await db.Squad.Where(s => s.Id == id).FirstOrDefaultAsync(ct);
            if (squad == null) return ServiceResult<List<SquadMatchResponse>>.Fail(ResultStatus.NotFound);


            // check if the squad already has the hard limit of 5 squad matches and time elapsed is less than 2 minutes
            //if (squad.SquadMatchCount == 5 && timeElapsed.Minutes < 2)
            if (!forceRefresh && squad.SquadMatchCount == 5)
            {
                var firstSquadMatch = await db.SquadMatch.Where(sm => sm.SquadId == id).FirstOrDefaultAsync(ct);
                if (firstSquadMatch != null)
                {
                    var timeElapsed = DateTimeOffset.Now - firstSquadMatch.CreatedAt;
                    if (timeElapsed.TotalMinutes < 2)
                    {
                        squadMatches = await db.SquadMatch.Where(sm => sm.SquadId == id).Join(db.Match, sm => sm.MatchId, m => m.Id, (sm, m) =>
                           new SquadMatchResponse(sm.SquadId, sm.MatchId, sm.ReasonForAddition, m.QueueId, m.GameStart, m.GameEnd, m.DurationSeconds, m.Mode, m.GameType, m.MapId, sm.CreatedAt)).ToListAsync(ct);
                        return ServiceResult<List<SquadMatchResponse>>.Ok(squadMatches);
                    }
                }
            }

            //if (timeElapsed.Minutes < 2) Console.WriteLine("its been longer than 2 minutes");
            // get all squad members from squad
            var res = await GetAllMembersAsync(id, ct);
            if (!res.IsSuccessful) return ServiceResult<List<SquadMatchResponse>>.Fail(res.Status);
            var squadMembers = res.Value;

            var squadMemberIds = squadMembers.Select(sm => sm.Puuid).ToList();
            if (squadMemberIds.Count == 0) return ServiceResult<List<SquadMatchResponse>>.Fail(ResultStatus.NotFound);
            var firstSquadMemberId = squadMemberIds[0];

            // fetch 100 match ids for squadmember1 using their id (same as puuid)
            var resMatchIds = await rs.GetMatchIdsAsync(firstSquadMemberId, 100, ct);
            if (!resMatchIds.IsSuccessful) return ServiceResult<List<SquadMatchResponse>>.Fail(resMatchIds.Status);
            var matchIds = resMatchIds.Value; // THIS CAN CAUSE AN ISSUE IF MEMBER 1 IS A FRESH ACCOUNT 

            // ADD LOGIC TO LOOP THROUGH SQUAD IF SOMEONE DOESN'T HAVE MATCHES              

            var existingSquadMatchIds = await db.SquadMatch.AsNoTracking().Where(sm => sm.SquadId == id).Select(sm => sm.MatchId).ToHashSetAsync(ct);

            var squadMatchesCount = squad.SquadMatchCount;


            if (squadMatchesCount == 5)
            {
                squad.SquadMatchCount = 0;
                squadMatchesCount = 0;
                await db.SaveChangesAsync(ct);
                await db.SquadMatch.Where(sm => sm.SquadId == id).ExecuteDeleteAsync(ct);
            }

            // for every match id, fetch the riot match, then check if every squadmemberid is in the riot match's particpant id list
            foreach (var matchId in matchIds)
            {
                if (squadMatchesCount >= 5)
                {
                    break;
                }
                if (existingSquadMatchIds.Contains(matchId)) continue;

                var resMatch = await rs.GetMatchAsync(matchId, ct);
                if (!resMatch.IsSuccessful)
                {
                    continue;
                }
                var match = resMatch.Value;

                bool addMatch = true;

                // put participant puuids in map for constant time access 
                Dictionary<string, int> participantIdsMap = new Dictionary<string, int>();
                foreach (var participantPuuid in match.ParticipantsIds)
                {
                    if (!participantIdsMap.ContainsKey(participantPuuid))
                    {
                        participantIdsMap.Add(participantPuuid, 0);
                    }
                }

                // make sure every squadmember is a participant of the current match
                foreach (var squadMemberId in squadMemberIds)
                {
                    // put and 
                    if (!participantIdsMap.ContainsKey(squadMemberId))
                    {
                        addMatch = false;
                        break;
                    }
                }

                if (addMatch)
                {

                    await using var tx = await db.Database.BeginTransactionAsync(ct);
                    try
                    {
                        // check is match is already in db
                        MatchResponse matchNeeded;

                        var existingMatch = await db.Match.FindAsync(matchId, ct);
                        if (existingMatch != null)
                        {
                            var m = await db.Match.Where(m => m.Id == matchId).Select(match =>
                                new MatchResponse(existingMatch.Id, existingMatch.QueueId, existingMatch.GameStart, existingMatch.GameEnd, existingMatch.DurationSeconds, existingMatch.Mode, existingMatch.GameType, existingMatch.MapId, existingMatch.CreatedAt)).FirstOrDefaultAsync(ct);
                            matchNeeded = m;
                        }
                        else
                        {
                            // add match to match table using match service
                            var resMatchCreated = await ms.AddAsync(match.MatchId, match.QueueId, match.GameStart, match.GameEnd, match.DurationSeconds, match.Mode, match.GameType, match.MapId, ct);
                            if (!resMatchCreated.IsSuccessful) return ServiceResult<List<SquadMatchResponse>>.Fail(resMatchCreated.Status);
                            var matchCreated = resMatchCreated.Value;
                            matchNeeded = matchCreated;
                        }

                        // add squadmatch to squadmatch table using squadmatch service
                        var resSquadMatchCreated = await sms.AddAsync(id, match.MatchId, "Match contains all Squad Members", matchNeeded, ct);
                        if (!resSquadMatchCreated.IsSuccessful) return ServiceResult<List<SquadMatchResponse>>.Fail(resSquadMatchCreated.Status);

                        var existingParticipantIds = await db.Participant.AsNoTracking().Where(p => p.MatchId == matchId).Select(x => new { x.MatchId, x.ParticipantId }).ToHashSetAsync(ct);


                        var existingBySlot = await db.Participant.Where(p => p.MatchId == match.MatchId).Select(p => new { p.ParticipantId, p.Puuid }).ToListAsync(ct);
                        var existingSlotSet = existingBySlot.Select(x => x.ParticipantId).ToHashSet();
                        var existingPuuidSet = existingBySlot.Select(x => x.Puuid).ToHashSet();


                        using var pDoc = JsonDocument.Parse(match.ParticipantsJson);
                        var rawByPuuid = pDoc.RootElement.EnumerateArray().ToDictionary(e => e.GetProperty("puuid").GetString()!, e => e.GetRawText());

                        var newParticipants = match.Participants.Select(p => new Participant
                        {
                            MatchId = match.MatchId,
                            ParticipantId = p.ParticipantId,
                            Puuid = p.Puuid,
                            TeamId = p.TeamId,
                            TeamPosition = p.TeamPosition,
                            ChampionId = p.ChampionId,
                            Kills = p.Kills,
                            Deaths = p.Deaths,
                            Assists = p.Assists,
                            Win = p.Win,
                            ParticipantsJson = rawByPuuid.TryGetValue(p.Puuid, out var x) ? x : "{}"
                        }).DistinctBy(x => new { x.MatchId, x.ParticipantId }).Where(x => !existingSlotSet.Contains(x.ParticipantId) && !existingPuuidSet.Contains(x.Puuid)).ToList();

                        if (newParticipants.Count > 0) await db.Participant.AddRangeAsync(newParticipants, ct);

                        MatchTimeline mt = new MatchTimeline() { Id = match.MatchId, TimelineJson = match.TimelineJson };
                        var existingMatchTimeline = await db.MatchTimeline.FindAsync(match.MatchId, ct);

                        if (existingMatchTimeline == null)
                        {
                            await db.MatchTimeline.AddAsync(mt, ct);
                        }

                        squad.SquadMatchCount += 1;
                        await db.SaveChangesAsync(ct);
                        await tx.CommitAsync(ct);



                        squadMatches.Add(resSquadMatchCreated.Value);
                        squadMatchesCount += 1;
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                        await tx.RollbackAsync(ct);
                        return ServiceResult<List<SquadMatchResponse>>.Fail(ResultStatus.Unknown);
                    }

                }
            }
            return ServiceResult<List<SquadMatchResponse>>.Ok(squadMatches);
        }
    }
}
