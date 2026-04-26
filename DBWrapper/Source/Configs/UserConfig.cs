using DBWrapper.Source.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBWrapper.Source.Configs
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.IsActive).HasDefaultValue(true);

            builder.HasOne(u => u.Loadout)
                .WithOne(l => l.User)
                .HasForeignKey<UserLoadout>(l => l.UserId);

            builder.HasOne(u => u.Stats)
                .WithOne(s => s.User)
                .HasForeignKey<UserStats>(s => s.UserId);
        }
    }
}
