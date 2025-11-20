using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IMatchService
    {
        Task<ServiceResult<MatchResponse>> AddAsync(string id, int queueId, DateTimeOffset gameStart, DateTimeOffset gameEnd, int durationSeconds, string mode, string gameType, int mapId, CancellationToken ct);

    }
}
