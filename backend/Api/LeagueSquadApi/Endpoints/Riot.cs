using LeagueSquadApi.Services.Interfaces;
using static LeagueSquadApi.Dtos.RiotDtos;


namespace LeagueSquadApi.Endpoints
{
    public static class Riot
    {
        public static void RegisterRiotEndpoints(this IEndpointRouteBuilder routes)
        {
            var riot = routes.MapGroup("/riot-account");

            // Find riot account (using tagline and game name) 
            riot.MapGet("/{gameName}/{tagLine}", async (string gameName, string tagLine, IRiotService rs, CancellationToken ct) =>
            {
                var res = await rs.GetAccountByRiotIdAsync(gameName, tagLine, ct);
                return ResultStatusToIResultMapper<RiotAccountResponse>.ToHttp(res);
            });

            // Find riot account (using puuid) 
            riot.MapGet("/{puuid}", async (string puuid, IRiotService rs, CancellationToken ct) =>
            {
                var res = await rs.GetAccountByPuuidAsync(puuid, ct);
                return ResultStatusToIResultMapper<RiotAccountResponse>.ToHttp(res);
            });
        }
    }
}
