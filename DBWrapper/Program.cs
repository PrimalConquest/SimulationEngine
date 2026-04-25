using DBWrapper.Source.Context;
using Microsoft.EntityFrameworkCore;
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

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PrimalConquestDbContext>();
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.Run();
        }
    }
}
