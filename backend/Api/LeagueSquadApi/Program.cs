using LeagueSquadApi.Endpoints;
using LeagueSquadApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterServices();

var app = builder.Build();

app.RegisterMiddlewares();

app.RegisterSquadEndpoints();

app.RegisterPlayerEndpoints();

app.RegisterRiotEndpoints();

app.MapGet("/", () => "Server is up!");

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    timeUtc = DateTime.UtcNow
}));

app.Run();



