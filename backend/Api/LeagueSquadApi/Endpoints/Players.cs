using LeagueSquadApi.Dtos;
using LeagueSquadApi.Services.Interfaces;

namespace LeagueSquadApi.Endpoints
{
    public static class Players
    {
        public static void RegisterPlayerEndpoints(this IEndpointRouteBuilder routes)
        {
            var players = routes.MapGroup("/players");

            // Get a Player 
            players.MapGet("/{id}", async (string id, IPlayerService ps, CancellationToken ct) =>
            {
                var res = await ps.GetAsync(id, ct);
                return ResultStatusToIResultMapper<PlayerResponse>.ToHttp(res);
            });

            // Get all players in the system
            players.MapGet("", async (IPlayerService ps, CancellationToken ct) =>
            {
                var res = await ps.GetAllAsync(ct);
                return ResultStatusToIResultMapper<List<PlayerResponse>>.ToHttp(res);
            });
        }
    }
}
