using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DBWrapper.Source.Context
{
    public class PrimalConquestContextFactory : IDesignTimeDbContextFactory<PrimalConquestDbContext>
    {
        public PrimalConquestDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PrimalConquestDbContext>();
            optionsBuilder.UseNpgsql("Host = localhost; Database = AppDbDev; Username = postgres; Password = postgres");
            return new PrimalConquestDbContext(optionsBuilder.Options);
        }
    }
}
