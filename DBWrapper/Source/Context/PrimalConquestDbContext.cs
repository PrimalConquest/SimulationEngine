using DBWrapper.Source.Configs;
using DBWrapper.Source.Models;
using Microsoft.EntityFrameworkCore;

namespace DBWrapper.Source.Context
{
    public class PrimalConquestDbContext : DbContext
    {
        public PrimalConquestDbContext(DbContextOptions<PrimalConquestDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.ApplyConfiguration(new UserConfig());
            mb.ApplyConfiguration(new UserInfoConfig());
        }
    }
}
