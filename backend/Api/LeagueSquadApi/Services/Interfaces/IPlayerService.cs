using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayerResponse?> UpsertPlayerWithRiotIdAsync(string gameName, string tagLine, CancellationToken ct);
        Task<PlayerResponse?> UpsertPlayerWithPuuidAsync(string puuid, CancellationToken ct);
    }
}
