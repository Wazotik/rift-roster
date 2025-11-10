using static LeagueSquadApi.Dtos.RiotDtos;
using LeagueSquadApi.Dtos;
namespace LeagueSquadApi.Services.Interfaces
{
    public interface IRiotService
    {
        Task<ServiceResult<RiotAccountResponse>> GetAccountByRiotIdAsync(string gameName, string tagLine, CancellationToken ct);
        Task<ServiceResult<RiotAccountResponse>> GetAccountByPuuidAsync(string puuid, CancellationToken ct);
        Task<ServiceResult<RiotMatchResponse>> GetMatchAsync(string id, CancellationToken ct);
        Task<ServiceResult<List<string>>> GetMatchIdsAsync(string puuid, int count, CancellationToken ct);
    }
}
