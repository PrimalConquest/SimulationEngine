using DBWrapper.Source.Configs;
using DBWrapper.Source.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DBWrapper.Source.Context
{
    public class PrimalConquestDbContext : IdentityDbContext<User>
    {
        public PrimalConquestDbContext(DbContextOptions<PrimalConquestDbContext> options) : base(options) { }

        public DbSet<UserLoadout> UserLoadouts { get; set; }
        public DbSet<UserStats> UserStats { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb); 
            mb.ApplyConfiguration(new UserConfig());
            mb.ApplyConfiguration(new UserLoadoutConfig());
            mb.ApplyConfiguration(new UserStatsConfig());
        }
    }
}
