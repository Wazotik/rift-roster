using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            return ServiceResult<SquadResponse>.Ok(new SquadResponse(squad.Id, squad.Name, squad.CreatedAt));
        }
        public async Task<ServiceResult<List<SquadResponse>>> GetAllAsync(CancellationToken ct)
        {
            var squads = await db.Squad.Select(s => new SquadResponse(s.Id, s.Name, s.CreatedAt)).ToListAsync(ct);
            return ServiceResult<List<SquadResponse>>.Ok(squads);
        }

        public async Task<ServiceResult<SquadResponse>> AddAsync(string name, CancellationToken ct)
        {
            Squad squad = new Squad() { Name = name };
            await db.Squad.AddAsync(squad, ct);
            await db.SaveChangesAsync(ct);
            return ServiceResult<SquadResponse>.Ok(new SquadResponse(squad.Id, squad.Name, squad.CreatedAt), ResultStatus.Created);
        }

        public async Task<ServiceResult<SquadResponse>> UpdateAsync(long id, string name, CancellationToken ct)
        {
            var squad = await db.Squad.Where(s => s.Id == id).FirstOrDefaultAsync(ct);
            if (squad == null) return ServiceResult<SquadResponse>.Fail(ResultStatus.NotFound);
            squad.Name = name;
            await db.SaveChangesAsync(ct);
            return ServiceResult<SquadResponse>.Ok(new SquadResponse(squad.Id, squad.Name, squad.CreatedAt));
        }

        public async Task<ServiceResult> DeleteAsync(long id, CancellationToken ct)
        {


            // delete squad members first
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
    }
}
