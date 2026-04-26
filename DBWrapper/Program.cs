using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AccountComunication;
using DBWrapper.Source.Context;
using DBWrapper.Source.Models;
using DBWrapper.Source.Models.Mappers;
using LoadoutComunication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedServices;
using SharedServices.Auth;
using SharedUtils.Source;

namespace DBWrapper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string host        = EnviromentProperty.Get("DB_HOST",          false);
            string port        = EnviromentProperty.Get("DB_PORT",          false);
            string dbName      = EnviromentProperty.Get("DB_NAME",          false);
            string dbUser      = EnviromentProperty.Get("DB_USER",          false);
            string dbPass      = EnviromentProperty.Get("DB_PASSWORD",      false);
            string jwtSecret   = EnviromentProperty.Get("JWT_SECRET",       false);
            string jwtIssuer   = EnviromentProperty.Get("JWT_ISSUER",       false);
            string jwtAudience = EnviromentProperty.Get("JWT_AUDIENCE",     false);
            string internalKey = EnviromentProperty.Get("INTERNAL_API_KEY", false);

            string connStr = $"Host={host};Port={port};Database={dbName};Username={dbUser};Password={dbPass}";

            var builder = WebApplication.CreateBuilder(args);

            // ── Database ─────────────────────────────────────────────────────────
            builder.Services.AddDbContext<PrimalConquestDbContext>(
                opts => opts.UseNpgsql(connStr));

            // ── Identity ─────────────────────────────────────────────────────────
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit           = true;
                options.Password.RequiredLength         = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase       = false;
                options.User.RequireUniqueEmail         = true;
            })
            .AddEntityFrameworkStores<PrimalConquestDbContext>()
            .AddDefaultTokenProviders();

            // ── JWT ───────────────────────────────────────────────────────────────
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
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
            });

            // ── Authorization policies ────────────────────────────────────────────
            var internalSettings = new InternalServiceSettings { ApiKey = internalKey };
            builder.Services.AddSingleton(internalSettings);
            builder.Services.AddSingleton<IAuthorizationHandler, InternalOrJwtHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, InternalOnlyHandler>();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(InternalOrJwtRequirement.Policy, p =>
                    p.AddRequirements(new InternalOrJwtRequirement()));
                options.AddPolicy(InternalOnlyRequirement.Policy, p =>
                    p.AddRequirements(new InternalOnlyRequirement()));
            });

            builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
                p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            var app = builder.Build();

            app.UseCors();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PrimalConquestDbContext>();
                db.Database.Migrate();
            }

            // ── Helpers ───────────────────────────────────────────────────────────
            string BuildJwt(User user)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,        user.Id),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                    new Claim(JwtRegisteredClaimNames.Email,      user.Email    ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
                };
                var token = new JwtSecurityToken(
                    issuer:             jwtIssuer,
                    audience:           jwtAudience,
                    claims:             claims,
                    expires:            DateTime.UtcNow.AddHours(24),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            // Returns the raw token (sent to client) and the entity (stored in DB).
            (string raw, RefreshToken entity) BuildRefreshToken(string userId)
            {
                var raw  = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
                return (raw, new RefreshToken
                {
                    UserId    = userId,
                    TokenHash = hash,
                    ExpiresAt = DateTime.UtcNow.AddDays(30),
                    CreatedAt = DateTime.UtcNow,
                });
            }

            AuthResponseDTO BuildAuthResponse(User user, string rawRefreshToken) => new()
            {
                AccessToken  = BuildJwt(user),
                RefreshToken = rawRefreshToken,
                UserId       = user.Id,
                UserName     = user.UserName ?? "",
            };

            // ── Health ────────────────────────────────────────────────────────────
            app.MapGet("/healthz", () => Results.Ok("healthy"));

            // ── Auth ──────────────────────────────────────────────────────────────
            app.MapPost("/auth/register", async (
                RegisterDTO dto,
                UserManager<User> userManager,
                PrimalConquestDbContext db) =>
            {
                var user   = UserMapper.FromDTO(dto);
                var result = await userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    return Results.BadRequest(result.Errors.Select(e => e.Description));

                var (raw, token) = BuildRefreshToken(user.Id);
                db.RefreshTokens.Add(token);
                db.UserLoadouts.Add(new UserLoadout { UserId = user.Id });
                db.UserStats.Add(new UserStats { UserId = user.Id });
                await db.SaveChangesAsync();

                return Results.Ok(BuildAuthResponse(user, raw));
            });

            app.MapPost("/auth/login", async (
                LoginDTO dto,
                UserManager<User> userManager,
                PrimalConquestDbContext db) =>
            {
                var user = await userManager.FindByEmailAsync(dto.Email);
                if (user is null || !await userManager.CheckPasswordAsync(user, dto.Password))
                    return Results.Unauthorized();

                // Sweep any stale sessions for this user before issuing a new one
                await db.RefreshTokens
                    .Where(t => t.UserId == user.Id)
                    .ExecuteDeleteAsync();

                var (raw, token) = BuildRefreshToken(user.Id);
                db.RefreshTokens.Add(token);
                await db.SaveChangesAsync();

                return Results.Ok(BuildAuthResponse(user, raw));
            });

            app.MapPost("/auth/refresh", async (
                RefreshRequestDTO dto,
                UserManager<User> userManager,
                PrimalConquestDbContext db) =>
            {
                var hash     = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(dto.RefreshToken)));
                var existing = await db.RefreshTokens
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.TokenHash == hash);

                // Invalid or expired — clean up and reject
                if (existing is null || existing.ExpiresAt <= DateTime.UtcNow)
                {
                    if (existing is not null) db.RefreshTokens.Remove(existing);
                    await db.SaveChangesAsync();
                    return Results.Unauthorized();
                }

                var user = existing.User;

                // Rotate: delete all tokens for this user and issue a fresh pair
                await db.RefreshTokens
                    .Where(t => t.UserId == user.Id)
                    .ExecuteDeleteAsync();

                var (raw, token) = BuildRefreshToken(user.Id);
                db.RefreshTokens.Add(token);
                await db.SaveChangesAsync();

                return Results.Ok(BuildAuthResponse(user, raw));
            });

            // Logout is public — the refresh token is the credential, no JWT needed
            app.MapPost("/auth/logout", async (
                RefreshRequestDTO dto,
                PrimalConquestDbContext db) =>
            {
                var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(dto.RefreshToken)));
                await db.RefreshTokens
                    .Where(t => t.TokenHash == hash)
                    .ExecuteDeleteAsync();
                return Results.Ok();
            });

            // ── Loadout ───────────────────────────────────────────────────────────
            app.MapGet("/loadout/{userId}", async (
                string userId,
                ClaimsPrincipal principal,
                PrimalConquestDbContext db) =>
            {
                var callerId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (callerId is not null && callerId != userId)
                    return Results.Forbid();

                var loadout = await db.UserLoadouts.FirstOrDefaultAsync(x => x.UserId == userId);
                return loadout is null
                    ? Results.NotFound($"No loadout for player '{userId}'")
                    : Results.Ok(UserLoadoutMapper.ToDTO(loadout));
            })
            .RequireAuthorization(InternalOrJwtRequirement.Policy);

            app.MapPut("/loadout/{userId}", async (
                string userId,
                LoadoutDTO dto,
                ClaimsPrincipal principal,
                PrimalConquestDbContext db) =>
            {
                var callerId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (callerId is not null && callerId != userId)
                    return Results.Forbid();

                var existing = await db.UserLoadouts.FirstOrDefaultAsync(x => x.UserId == userId);
                if (existing is null)
                    return Results.NotFound($"No loadout for player '{userId}'");

                existing.CommanderId = dto.CommanderId;
                existing.OfficerIds  = dto.OfficerIds;
                await db.SaveChangesAsync();

                return Results.Ok(UserLoadoutMapper.ToDTO(existing));
            })
            .RequireAuthorization(InternalOrJwtRequirement.Policy);

            // ── Stats ─────────────────────────────────────────────────────────────
            app.MapGet("/stats/{userId}", async (
                string userId,
                PrimalConquestDbContext db) =>
            {
                var stats = await db.UserStats.FirstOrDefaultAsync(x => x.UserId == userId);
                return stats is null
                    ? Results.NotFound($"No stats for player '{userId}'")
                    : Results.Ok(UserStatsMapper.ToDTO(stats));
            })
            .RequireAuthorization(InternalOrJwtRequirement.Policy);

            // Only internal services (BattleServer after match) can write rank
            app.MapPut("/stats/{userId}", async (
                string userId,
                UserStatsDTO dto,
                PrimalConquestDbContext db) =>
            {
                var stats = await db.UserStats.FirstOrDefaultAsync(x => x.UserId == userId);
                if (stats is null)
                    return Results.NotFound($"No stats for player '{userId}'");

                stats.RankPoints = dto.RankPoints;
                await db.SaveChangesAsync();

                return Results.Ok(UserStatsMapper.ToDTO(stats));
            })
            .RequireAuthorization(InternalOnlyRequirement.Policy);

            app.Run();
        }
    }
}
