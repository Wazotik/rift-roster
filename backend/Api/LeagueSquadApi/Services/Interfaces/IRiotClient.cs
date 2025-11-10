using static LeagueSquadApi.Dtos.RiotDtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IRiotClient
    {
        Task<RiotHttpResult<RiotAccountResponse>> GetAccountByRiotIdAsync(string gameName, string tagLine, CancellationToken ct);
        Task<RiotHttpResult<RiotAccountResponse>> GetAccountByPuuidAsync(string puuid, CancellationToken ct);
        Task<RiotHttpResult<List<string>>> GetMatchIdsAsync(string puuid, int count, CancellationToken ct);
        Task<RiotHttpResult<RiotMatchResponse>> GetMatchAsync(string id, CancellationToken ct);
    }
}
