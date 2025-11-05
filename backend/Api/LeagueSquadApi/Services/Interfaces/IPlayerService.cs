using LeagueSquadApi.Data.Models;

namespace LeagueSquadApi.Services.Interfaces
{
    public interface IPlayerService
    {

        Task<Player?> UpsertPlayerWithRiotId(string gameName, string tagLine, CancellationToken ct);
        Task<Player?> UpsertPlayerWithPuuid(string puuid, CancellationToken ct);

    }
}
