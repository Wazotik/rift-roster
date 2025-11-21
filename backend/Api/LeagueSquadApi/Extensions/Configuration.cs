using LeagueSquadApi.Data;
using LeagueSquadApi.Services;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeagueSquadApi.Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            var connectionString =
                Environment.GetEnvironmentVariable("DB_CONN_STRING")
                ?? builder.Configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException("Missing db conn string");

            builder.Services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(connectionString);
            });

            var riotApiKey =
                Environment.GetEnvironmentVariable("RIOT_API_KEY")
                ?? builder.Configuration["RiotApiKey"]
                ?? throw new InvalidOperationException("Missing riot api key");

            builder.Services.AddHttpClient<IRiotClient, RiotClient>(http =>
            {
                http.BaseAddress = new Uri("https://americas.api.riotgames.com/");
                http.DefaultRequestHeaders.Add("X-Riot-Token", riotApiKey);
                http.Timeout = TimeSpan.FromSeconds(10);
            });
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<ISquadService, SquadService>();
            builder.Services.AddScoped<ISquadMatchService, SquadMatchService>();
            builder.Services.AddScoped<IMatchService, MatchService>();
            builder.Services.AddScoped<IRiotService, RiotService>();
            builder.Services.AddScoped<IParticipantService, ParticipantService>();
            builder.Services.AddScoped<IMatchAggregatedStatsService, MatchAggregatedStatsService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(
                    "frontend",
                    p =>
                    {
                        var allowedOrigins = new[]
                        {
                            "http://localhost:5173",
                            "https://riftroster.netlify.app", // update with actual Netlify URL
                            "https://www.riftroster.netlify.app",
                        };

                        p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
                    }
                );
            });
        }

        public static void RegisterMiddlewares(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("frontend");
        }
    }
}
