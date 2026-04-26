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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

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

            builder.Services.AddDbContext<PrimalConquestDbContext>(
                opts => opts.UseNpgsql(connStr));

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit          = true;
                options.Password.RequiredLength        = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase      = false;
                options.User.RequireUniqueEmail        = true;
            })
            .AddEntityFrameworkStores<PrimalConquestDbContext>()
            .AddDefaultTokenProviders();

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

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PrimalConquestDbContext>();
                db.Database.Migrate();
            }

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

            
            app.MapGet("/healthz", () => Results.Ok("healthy"));


            app.MapPost("/auth/register", async (
                RegisterDTO dto,
                UserManager<User> userManager,
                PrimalConquestDbContext db) =>
            {
                var user   = UserMapper.FromDTO(dto);
                var result = await userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    return Results.BadRequest(result.Errors.Select(e => e.Description));

                db.UserLoadouts.Add(new UserLoadout { UserId = user.Id });
                db.UserStats.Add(new UserStats { UserId = user.Id });
                await db.SaveChangesAsync();

                return Results.Ok(new AuthResponseDTO
                {
                    Token    = BuildJwt(user),
                    UserId   = user.Id,
                    UserName = user.UserName ?? "",
                });
            });

            app.MapPost("/auth/login", async (
                LoginDTO dto,
                UserManager<User> userManager) =>
            {
                var user = await userManager.FindByEmailAsync(dto.Email);
                if (user is null || !await userManager.CheckPasswordAsync(user, dto.Password))
                    return Results.Unauthorized();

                return Results.Ok(new AuthResponseDTO
                {
                    Token    = BuildJwt(user),
                    UserId   = user.Id,
                    UserName = user.UserName ?? "",
                });
            });

            
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
