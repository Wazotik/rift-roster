using Microsoft.EntityFrameworkCore;
using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Services;
using LeagueSquadApi.Services.Interfaces;

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

// Get all players in the system
app.MapGet("/players", async (AppDbContext db, CancellationToken ct) =>
{
    var allPlayers = await db.Player.Select(p => new PlayerResponse(p.Id, p.GameName, p.TagLine, p.Region, p.CreatedAt)).ToListAsync(ct);
    return allPlayers.Any() == false ? Results.NotFound() : Results.Ok(allPlayers);
});

// Get a Player 

app.MapGet("/players/{id}", async (string id, AppDbContext db, CancellationToken ct) =>
{
    var p = await db.Player.Where(p => p.Id == id).FirstOrDefaultAsync(ct);
    return p == null ? Results.NotFound() : Results.Ok(new PlayerResponse(p.Id, p.GameName, p.TagLine, p.Region, p.CreatedAt));
});




// Squad Routes

// Create a squad
app.MapPost("/squads", async (SquadRequest req, AppDbContext db, CancellationToken ct) =>
{
    Squad newSquad = new Squad() { Name = req.Name };
    await db.AddAsync(newSquad, ct);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/squads/{newSquad.Id}", new SquadResponse(newSquad.Id, newSquad.Name, newSquad.CreatedAt));
});


// Get squad using id
app.MapGet("/squads/{id}", async (long id, AppDbContext db, CancellationToken ct) =>
{
    var squad = await db.Squad.Where(s => s.Id == id).FirstOrDefaultAsync(ct);
    return squad == null ? Results.NotFound() : Results.Ok(new SquadResponse(squad.Id, squad.Name, squad.CreatedAt));
});

// Get all squads 
app.MapGet("/squads", async (AppDbContext db, CancellationToken ct) =>
{
    var allSquads = await db.Squad.Select(s => new SquadResponse(s.Id, s.Name, s.CreatedAt)).ToListAsync(ct);
    return allSquads == null ? Results.NotFound() : Results.Ok(allSquads);
});


// Update a squads details
app.MapPut("/squads/{id}", async (long id, SquadRequest req, AppDbContext db, CancellationToken ct) =>
{
    var squad = await db.Squad.Where(s => s.Id == id).FirstOrDefaultAsync(ct);
    if (squad != null)
    {
        squad.Name = req.Name;
        await db.SaveChangesAsync(ct);
    }
    else
    {
        return Results.NotFound();
    }

    return Results.Ok(new SquadResponse(squad.Id, squad.Name, squad.CreatedAt));
});


// Get all members of a squad
app.MapGet("/squads/{id}/members", async (long id, AppDbContext db, CancellationToken ct) =>
{
    var squadMembers = await db.SquadMember.Where(sm => sm.SquadId == id).Join(db.Player, sm => sm.Puuid, p => p.Id, (sm, p) =>
        new SquadMemberResponse(sm.SquadId, sm.Puuid, sm.Role ?? "", sm.Alias ?? "", sm.CreatedAt, p.GameName, p.TagLine, p.Region ?? "")).ToListAsync(ct);
    return squadMembers.Any() == false ? Results.NotFound() : Results.Ok(squadMembers);
});


// Add a member to a squad
app.MapPost("/squads/{id}/members", async (long id, SquadMemberRequest req, IPlayerService ps, AppDbContext db, CancellationToken ct) =>
{
    var p = await ps.UpsertPlayerWithPuuidAsync(req.Puuid, ct);
    if (p == null) return Results.NotFound(ct);
    SquadMember sm = new SquadMember() { SquadId = id, Puuid = req.Puuid, Role = req.Role, Alias = req.Alias };
    await db.SquadMember.AddAsync(sm, ct);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/squads{id}/members/{sm.Puuid}", new SquadMemberResponse(sm.SquadId, sm.Puuid, sm.Role ?? "", sm.Alias ?? "", sm.CreatedAt, p.GameName, p.TagLine, p.Region ?? ""));
});


// Get a member from a squad
app.MapGet("/squads/{id}/members{puuid}", async (long id, string puuid, AppDbContext db, CancellationToken ct) =>
{
    var sm = await db.SquadMember.Where(sm => sm.Puuid == puuid && sm.SquadId == id).FirstOrDefaultAsync(ct);

    return sm == null ? Results.NotFound() : Results.Ok(sm);
});



// Delete a member from a squad



















app.Run();



