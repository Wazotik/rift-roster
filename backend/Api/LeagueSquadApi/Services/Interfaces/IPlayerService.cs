using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<ServiceResult<PlayerResponse>> GetAsync(string id, CancellationToken ct);
        Task<ServiceResult<List<PlayerResponse>>> GetAllAsync(CancellationToken ct);
        Task<ServiceResult<PlayerResponse>> UpsertWithRiotIdAsync(string gameName, string tagLine, CancellationToken ct);
        Task<ServiceResult<PlayerResponse>> UpsertWithPuuidAsync(string puuid, CancellationToken ct);
    }
}
