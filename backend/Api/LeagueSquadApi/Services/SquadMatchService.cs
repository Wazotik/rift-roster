using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services.Interfaces;

namespace LeagueSquadApi.Services
{
    public class SquadMatchService : ISquadMatchService
    {
        private readonly AppDbContext db;

        public SquadMatchService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<ServiceResult<SquadMatchResponse>> AddAsync(long squadId, string matchId, string? ReasonForAddition, MatchResponse mr, CancellationToken ct)
        {
            SquadMatch sm = new SquadMatch() { SquadId = squadId, MatchId = matchId, ReasonForAddition = ReasonForAddition };
            await db.AddAsync(sm, ct);
            await db.SaveChangesAsync(ct);
            return ServiceResult<SquadMatchResponse>.Ok(new SquadMatchResponse(sm.SquadId, sm.MatchId, sm.ReasonForAddition, mr.QueueId, mr.GameStart, mr.GameEnd, mr.DurationSeconds, mr.Mode, mr.GameType, mr.MapId, sm.CreatedAt), ResultStatus.Created);
        }
    }
}
