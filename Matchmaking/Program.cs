using System.Text;
using Matchmaking.Source.Hubs;
using Matchmaking.Source.Queue;
using Matchmaking.Source.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SharedServices;
using SharedServices.Auth;
using SharedUtils.Source;

namespace Matchmaking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string dbWrapperUrl = EnviromentProperty.Get("DB_WRAPPER_URL",  false);
            string jwtSecret    = EnviromentProperty.Get("JWT_SECRET",       false);
            string jwtIssuer    = EnviromentProperty.Get("JWT_ISSUER",       false);
            string jwtAudience  = EnviromentProperty.Get("JWT_AUDIENCE",     false);
            string internalKey  = EnviromentProperty.Get("INTERNAL_API_KEY", false);

            var builder = WebApplication.CreateBuilder(args);

            // ── CORS ──────────────────────────────────────────────────────────────
            builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
                p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            // ── JWT ───────────────────────────────────────────────────────────────
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

            // ── SignalR ───────────────────────────────────────────────────────────
            builder.Services.AddSignalR();

            // ── Internal auth settings (shared with SharedServices middleware) ────
            builder.Services.AddSingleton(new InternalServiceSettings { ApiKey = internalKey });

            // ── DBWrapper HTTP client ─────────────────────────────────────────────
            builder.Services.AddHttpClient<DbWrapperClient>(c =>
            {
                c.BaseAddress = new Uri(dbWrapperUrl);
                c.DefaultRequestHeaders.Add("X-Internal-Key", internalKey);
            });

            // ── Matchmaking services ──────────────────────────────────────────────
            builder.Services.AddSingleton<MatchmakingQueue>();
            builder.Services.AddSingleton<AgonesAllocator>();
            builder.Services.AddHostedService<MatchmakingBackgroundService>();

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/healthz", () => Results.Ok("healthy"));
            app.MapHub<MatchmakingHub>("/queue");

            app.Run();
        }
    }
}
