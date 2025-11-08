using Microsoft.EntityFrameworkCore;
using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Dtos.Enums;
using LeagueSquadApi.Services;
using LeagueSquadApi.Services.Interfaces;
using LeagueSquadApi.Endpoints;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);
var riotApiKey = builder.Configuration["RiotApiKey"];
var connectionString = builder.Configuration.GetConnectionString("Postgres");


// Services
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(connectionString);
});

builder.Services.AddHttpClient<IRiotClient, RiotClient>(http =>
{
    http.BaseAddress = new Uri("https://americas.api.riotgames.com/riot/");
    http.DefaultRequestHeaders.Add("X-Riot-Token", riotApiKey);
    http.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ISquadService, SquadService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: allow your React dev server to call the API from a different origin
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("frontend", p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();



// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Endpoints
app.MapGet("/", () => "Server is up!");

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    timeUtc = DateTime.UtcNow
}));

// Find riot account (using tagline and game name) 
app.MapGet("/riot-account/{gameName}/{tagLine}", async (string gameName, string tagLine, IRiotClient riotClient, CancellationToken ct) =>
{
    var account = await riotClient.GetAccountByRiotIdAsync(gameName, tagLine, ct);
    return Results.Ok(account);
});

// Find riot account (using puuid) 
app.MapGet("/riot-account/{puuid}", async (string puuid, IRiotClient riotClient, CancellationToken ct) =>
{
    var account = await riotClient.GetAccountByPuuidAsync(puuid, ct);
    return Results.Ok(account);
});


// Player routes

// Get a Player 
app.MapGet("/players/{id}", async (string id, IPlayerService ps, CancellationToken ct) =>
{
    var res = await ps.GetAsync(id, ct);
    return ResultStatusToIResultMapper<PlayerResponse>.ToHttp(res);
});

// Get all players in the system
app.MapGet("/players", async (IPlayerService ps, CancellationToken ct) =>
{
    var res = await ps.GetAllAsync(ct);
    return ResultStatusToIResultMapper<List<PlayerResponse>>.ToHttp(res);
});





// Squad Routes

// Create a squad
app.MapPost("/squads", async (SquadRequest req, ISquadService ss, CancellationToken ct) =>
{
    var res = await ss.AddAsync(req.Name, ct);
    return ResultStatusToIResultMapper<SquadResponse>.ToHttp(res, $"/squads/{res?.Value?.Id}");
});


// Get squad using id
app.MapGet("/squads/{id}", async (long id, ISquadService ss, CancellationToken ct) =>
{
    var res = await ss.GetAsync(id, ct);
    return ResultStatusToIResultMapper<SquadResponse>.ToHttp(res);
});

// Get all squads 
app.MapGet("/squads", async (ISquadService ss, CancellationToken ct) =>
{
    var res = await ss.GetAllAsync(ct);
    return ResultStatusToIResultMapper<List<SquadResponse>>.ToHttp(res);
});

// Update a squads details
app.MapPut("/squads/{id}", async (long id, ISquadService ss, SquadRequest req, CancellationToken ct) =>
{
    var res = await ss.UpdateAsync(id, req.Name, ct);
    return ResultStatusToIResultMapper<SquadResponse>.ToHttp(res);
});

// Delete a squad 
app.MapDelete("/squads/{id}", async (long id, ISquadService ss, CancellationToken ct) =>
{
    var res = await ss.DeleteAsync(id, ct);
    return ResultStatusToIResultMapper.ToHttp(res);
});

// Get all members of a squad
app.MapGet("/squads/{id}/members", async (long id, ISquadService ss, CancellationToken ct) =>
{
    var res = await ss.GetAllMembersAsync(id, ct);
    return ResultStatusToIResultMapper<List<SquadMemberResponse>>.ToHttp(res);
});

// Add a member to a squad
app.MapPost("/squads/{id}/members", async (long id, SquadMemberRequest req, IPlayerService ps, ISquadService ss, CancellationToken ct) =>
{
    var res = await ss.AddMemberAsync(id, req, ps, ct);
    return ResultStatusToIResultMapper<SquadMemberResponse>.ToHttp(res, $"/squads/{id}/members/{res?.Value?.Puuid}");
});

// Get a member from a squad
app.MapGet("/squads/{id}/members/{puuid}", async (long id, string puuid, ISquadService ss, CancellationToken ct) =>
{
    var res = await ss.GetMemberAsync(id, puuid, ct);
    return ResultStatusToIResultMapper<SquadMemberResponse>.ToHttp(res);
});


// Delete a member from a squad
app.MapDelete("/squads/{id}/members/{puuid}", async (long id, string puuid, ISquadService ss, CancellationToken ct) =>
{
    var res = await ss.DeleteMemberAsync(id, puuid, ct);
    return ResultStatusToIResultMapper.ToHttp(res);
});















app.Run();



