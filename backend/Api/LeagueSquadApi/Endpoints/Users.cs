using LeagueSquadApi.Dtos;

namespace LeagueSquadApi.Endpoints
{
    public static class Users
    {
        //Extension method: class + method must be static, and parameter needs "this" keyword to signal what we are extending

        public static void RegisterUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var users = routes.MapGroup("/users").RequireAuthorization();

            //users.MapGet("/{id}", async (int id, IUserService us, CancellationToken ct) =>
            //{
            //    var token = await ls.AuthenticateUserAsync(req.Username, req.Password);
            //    return ResultStatusToIResultMapper<string>.ToHttp()
            //});
        }

    }
}
