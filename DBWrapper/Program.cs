using DBWrapper.Source.Context;
using DBWrapper.Source.Models.Mappers;
using LoadoutComunication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SharedServices;
using SharedUtils.Source;

namespace DBWrapper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string host = EnviromentProperty.Get("DB_HOST", false);
            string port = EnviromentProperty.Get("DB_PORT", false);
            string dbName = EnviromentProperty.Get("DB_NAME", false);
            string dbUser = EnviromentProperty.Get("DB_USER", false);
            string dbPass = EnviromentProperty.Get("DB_PASSWORD", false);

            string connStr = $"Host={host};Port={port};Database={dbName};Username={dbUser};Password={dbPass}";

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<PrimalConquestDbContext>(opts => opts.UseNpgsql(connStr));

            //builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PrimalConquestDbContext>();
                db.Database.Migrate();
            }
            //app.UseAuthorization();
            app.MapGet("/healthz", () => Results.Ok("healthy"));
            app.MapGet("/routes", (IEnumerable<EndpointDataSource> sources) =>
            {
                return sources
                    .SelectMany(s => s.Endpoints)
                    .Select(e => e.DisplayName);
            });


            app.MapPut("/account/register", async (AccountDTO dto, PrimalConquestDbContext db) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.UserName == dto.UserName);
                if (user is not null)
                    return Results.Conflict($"Player alredy exist '{user.UserName}'");


                user = UserMapper.FromDTO(dto);
                db.Users.Add(user);
                await db.SaveChangesAsync();

                return Results.Ok("registerd");
            });

            app.MapGet("/account/login", async (AccountDTO dto, PrimalConquestDbContext db) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.UserName == dto.UserName);
                if (user is null)
                    return Results.BadRequest($"No such player '{dto.UserName}'");

                if(user.Password != dto.Password)
                    return Results.BadRequest($"Invalid Credentials '{dto.UserName}'");

                return Results.Ok("loged in");
            });


            app.MapGet("/loadout/get/{playerId:int}", async (int playerId, PrimalConquestDbContext db) =>
            {
                var loadout = await db.UserInfos.FirstOrDefaultAsync(x => x.UserId == playerId);
                return loadout is null
                    ? Results.BadRequest($"No loadout found for player '{playerId}'")
                    : Results.Ok( UserInfoMapper.ToDTO(loadout));
            });

            app.MapPut("/loadout/put/{playerId:int}", async (LoadoutDTO loadout, PrimalConquestDbContext db) =>
            {
                var existing = await db.UserInfos.FirstOrDefaultAsync(x => x.UserId == loadout.PlayerId);
                if (existing is null)
                {
                    db.UserInfos.Add(UserInfoMapper.FromDTO(loadout));
                }
                else
                {
                    existing.CommanderId = loadout.CommanderId;
                    existing.OfficerIds = loadout.OfficerIds;
                }
                await db.SaveChangesAsync();
                return Results.Ok(loadout);
            });

            app.Run();

        }
    }
}
