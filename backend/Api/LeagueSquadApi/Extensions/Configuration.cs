using LeagueSquadApi.Data;
using Microsoft.EntityFrameworkCore;
using LeagueSquadApi.Services;
using LeagueSquadApi.Services.Interfaces;

namespace LeagueSquadApi.Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            var riotApiKey = builder.Configuration["RiotApiKey"] ?? throw new InvalidOperationException("Missing riot api key");
            var connectionString = builder.Configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Missing db conn string");

            builder.Services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(connectionString);
            });
            builder.Services.AddHttpClient<IRiotClient, RiotClient>(http =>
            {
                Console.WriteLine(riotApiKey);
                Console.WriteLine(connectionString);
                http.BaseAddress = new Uri("https://americas.api.riotgames.com/");
                http.DefaultRequestHeaders.Add("X-Riot-Token", riotApiKey);
                http.Timeout = TimeSpan.FromSeconds(10);
            });
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<ISquadService, SquadService>();
            builder.Services.AddScoped<ISquadMatchService, SquadMatchService>();
            builder.Services.AddScoped<IMatchService, MatchService>();
            builder.Services.AddScoped<IRiotService, RiotService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("frontend", p =>
                    p.WithOrigins("http://localhost:5173")
                     .AllowAnyHeader()
                     .AllowAnyMethod());
            });
        }

        public static void RegisterMiddlewares(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("frontend");
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}
