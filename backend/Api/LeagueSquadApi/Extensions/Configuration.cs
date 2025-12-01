using LeagueSquadApi.Data;
using LeagueSquadApi.Data.Models;
using LeagueSquadApi.Dtos;
using LeagueSquadApi.Services;
using LeagueSquadApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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


            // builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
            builder.Services.Configure<JwtOptions>(opts =>
            {
                opts.Key = jwtKey;
                opts.Issuer = jwtIssuer;
                opts.Audience = jwtAudience;
                opts.ExpiresMinutes = int.Parse(jwtSection["ExpiresMinutes"] ?? "10080"); // e.g. 7 days
            });

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                    };

                    opts.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.TryGetValue("access_token", out var token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"JWT failed: {ctx.Exception}");
                return Task.CompletedTask;
            }
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
            builder.Services.AddScoped<ISquadAggregatedStatsService, SquadAggregatedStatsService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearerAuth"
                            }
                        },
                        new string[] {}
                    },
                });

            });

            var allowedOriginsConfig = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")
                ?? builder.Configuration["AllowedOrigins"];

            var allowedOrigins = new List<string>
            {
                "http://localhost:5173",
                "https://riftroster.netlify.app",
                "https://www.riftroster.netlify.app",
            };

            if (!string.IsNullOrEmpty(allowedOriginsConfig))
            {
                var envOrigins = allowedOriginsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.Trim())
                    .Where(o => !string.IsNullOrEmpty(o));
                allowedOrigins.AddRange(envOrigins);
            }

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(
                    "frontend",
                    p =>
                    {
                        p.WithOrigins(allowedOrigins.Distinct().ToArray())
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials();
                    }
                );
            });
        }

        public static void RegisterMiddlewares(this WebApplication app)
        {
            app.UseCors("frontend");


            app.UseSwagger();
            app.UseSwaggerUI();

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
