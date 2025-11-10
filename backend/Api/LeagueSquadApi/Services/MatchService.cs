using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services.Interfaces;

namespace LeagueSquadApi.Services
{
    public class MatchService : IMatchService
    {
        private readonly AppDbContext db;

        public MatchService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<ServiceResult<MatchResponse>> AddAsync(string id, int queueId, DateTimeOffset gameStart, DateTimeOffset gameEnd, int durationSeconds, string mode, string gameType, int mapId, CancellationToken ct)
        {
            Match m = new Match() { Id = id, QueueId = queueId, GameStart = gameStart, GameEnd = gameEnd, DurationSeconds = durationSeconds, Mode = mode, GameType = gameType, MapId = mapId };
            await db.Match.AddAsync(m, ct);
            await db.SaveChangesAsync(ct);
            return ServiceResult<MatchResponse>.Ok(new MatchResponse(m.Id, m.QueueId, m.GameEnd, m.GameEnd, m.DurationSeconds, m.Mode, m.GameType, m.MapId, m.CreatedAt), ResultStatus.Created);
        }


    }
}
