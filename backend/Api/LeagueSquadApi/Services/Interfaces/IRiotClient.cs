using static LeagueSquadApi.Dtos.RiotDtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IRiotClient
    {
        Task<RiotAccountResponse?> GetAccountByRiotIdAsync(string gameName, string tagLine, CancellationToken ct);
        Task<RiotAccountResponse?> GetAccountByPuuidAsync(string puuid, CancellationToken ct);
    }
}
