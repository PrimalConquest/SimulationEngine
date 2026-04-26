using DBWrapper.Source.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBWrapper.Source.Configs
{
    public class UserStatsConfig : IEntityTypeConfiguration<UserStats>
    {
        public void Configure(EntityTypeBuilder<UserStats> builder)
        {
            builder.ToTable("UserStats");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).UseIdentityColumn();
            builder.Property(s => s.RankPoints).HasDefaultValue(0).IsRequired();
        }
    }
}
