using BattleServer.Source.Hubs;
using BattleServer.Source.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SimulationEngine.Source.Data.Geometry;
using SimulationEngine.Source.Logistic;
using System.Text;

namespace BattleServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dbWrapperUrl = Environment.GetEnvironmentVariable("DB_WRAPPER_URL")   ?? "";
            var internalKey  = Environment.GetEnvironmentVariable("INTERNAL_API_KEY") ?? "";
            var jwtSecret    = Environment.GetEnvironmentVariable("JWT_SECRET")        ?? "";
            var jwtIssuer    = Environment.GetEnvironmentVariable("JWT_ISSUER")        ?? "";
            var jwtAudience  = Environment.GetEnvironmentVariable("JWT_AUDIENCE")      ?? "";

            var builder = WebApplication.CreateBuilder(args);

            // ── JWT ────────────────────────────────────────────────────────────────
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.MapInboundClaims = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = jwtIssuer,
                    ValidAudience            = jwtAudience,
                    IssuerSigningKey         = signingKey,
                };

                // SignalR passes the token as a query-string parameter.
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        var token = ctx.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(token))
                            ctx.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            // ── SignalR ────────────────────────────────────────────────────────────
            builder.Services.AddSignalR();

            // ── Agones SDK ─────────────────────────────────────────────────────────
            builder.Services.AddSingleton<AgonesService>();
            builder.Services.AddHostedService<AgonesHealthWorker>();

            // ── DBWrapper client ───────────────────────────────────────────────────
            builder.Services.AddHttpClient<DbWrapperClient>(c =>
            {
                c.BaseAddress = new Uri(dbWrapperUrl);
                c.DefaultRequestHeaders.Add("X-Internal-Key", internalKey);
                c.Timeout     = TimeSpan.FromSeconds(10);
            });

            // ── Game instance ──────────────────────────────────────────────────────
            Game game = new(2, new Cell(7, 7));
            game.InitGame();
            builder.Services.AddSingleton(game);

            var app = builder.Build();

            app.UseWebSockets();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<SimulationHub>("/hub/simulation");

            app.Run();
        }
    }
}
