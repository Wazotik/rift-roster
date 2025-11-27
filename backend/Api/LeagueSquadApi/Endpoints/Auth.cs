using LeagueSquadApi.Dtos;
using LeagueSquadApi.Services.Interfaces;
using System.Security.Claims;

namespace LeagueSquadApi.Endpoints
{
    public static class Auth
    {
        public static void RegisterAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            var auth = routes.MapGroup("/auth");

            auth.MapPost("/login", async (LoginRequest req, HttpContext http, IAuthService authS, IUserService us, CancellationToken ct) =>
            {
                var res = await authS.LoginAsync(req, us, ct);
                if (res == null || res.Value == null) return Results.Unauthorized();
                string token = res.Value;
                if (token == null) return Results.Unauthorized();
                var cookieOptions = new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                };
                http.Response.Cookies.Append("access_token", token, cookieOptions);
                return Results.Ok();
            });

            auth.MapGet("/me", async (ClaimsPrincipal user, IUserService us, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Results.NotFound();
                var resUser = await us.GetWithIdAsync(int.Parse(userId), ct);
                if (resUser == null) return Results.NotFound();
                return Results.Ok(resUser.Value);
            });

            auth.MapPost("/logout", (HttpContext http, CancellationToken ct) =>
            {
                http.Response.Cookies.Delete("access_token");
                return Results.Ok();
            });

            auth.MapPost("/register", async (CreateUserRequest req, IAuthService authS, IUserService us, CancellationToken ct) =>
            {
                var res = await authS.RegisterAsync(req, us, ct);
                return ResultStatusToIResultMapper<UserResponse>.ToHttp(res);
            });
        }
    }
}
