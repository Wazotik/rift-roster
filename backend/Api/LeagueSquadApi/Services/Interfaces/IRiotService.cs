using static LeagueSquadApi.Dtos.RiotDtos;
using LeagueSquadApi.Dtos;
namespace LeagueSquadApi.Services.Interfaces
{
    public interface IRiotService
    {
        Task<ServiceResult<RiotAccountResponse>> GetAccountByRiotIdAsync(string gameName, string tagLine, IRiotClient rc, CancellationToken ct);
        Task<ServiceResult<RiotAccountResponse>> GetAccountByPuuidAsync(string puuid, IRiotClient rc, CancellationToken ct);
    }
}
