using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Services;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LeagueSquadApi.Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {

            if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("LocalDevelopment"))
            {
                builder.Configuration.AddUserSecrets<Program>(optional: true);
            }

            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? jwtSection["Key"] ?? throw new InvalidOperationException("Missing jwt secret key");
            var jwtIssuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Missing jwt issuer ");
            var jwtAudience = jwtSection["Audience"] ?? throw new InvalidOperationException("Missing jwt audience");


            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();

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

            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
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
            app.UseAuthentication();
            app.UseAuthorization();
        }

        public static async Task ApplyMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}
