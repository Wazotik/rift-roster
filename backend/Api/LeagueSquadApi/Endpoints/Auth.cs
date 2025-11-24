using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Services.Interfaces;

namespace LeagueSquadApi.Endpoints
{
    public static class Auth
    {
        public static void RegisterAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var auth = routes.MapGroup("/auth");

            auth.MapPost("/login", async (LoginRequest req, IAuthService authS, IUserService us, CancellationToken ct) =>
            {
                var res = await authS.LoginAsync(req, us, ct);
                return ResultStatusToIResultMapper<string>.ToHttp(res);
            });

            auth.MapPost("/register", async (CreateUserRequest req, IAuthService authS, IUserService us, CancellationToken ct) =>
            {
                var res = await authS.RegisterAsync(req, us, ct);
                return ResultStatusToIResultMapper<UserResponse>.ToHttp(res);
            });
        }
    }
}
