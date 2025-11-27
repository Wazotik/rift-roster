using LeagueSquadApi.Endpoints;
using LeagueSquadApi.Extensions;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterServices();

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.RegisterMiddlewares();

app.RegisterUserEndpoints();

app.RegisterAuthEndpoints();

app.RegisterSquadEndpoints();

app.RegisterPlayerEndpoints();

app.RegisterRiotEndpoints();

app.MapGet("/", () => "Server is up!");

app.MapGet("/health", () => Results.Ok(new { status = "ok", timeUtc = DateTime.UtcNow }));

app.MapGet("/me", (ClaimsPrincipal user) =>
{
    return Results.Ok(new { userEmail = user.FindFirstValue(ClaimTypes.Name) });
}).RequireAuthorization();

app.Run();
