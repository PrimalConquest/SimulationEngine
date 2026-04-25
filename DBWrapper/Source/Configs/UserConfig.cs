using DBWrapper.Source.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBWrapper.Source.Configs
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).UseIdentityColumn();

            builder.Property(l => l.UserName).IsRequired().HasMaxLength(50);
            builder.Property(l => l.Email).IsRequired().HasMaxLength(250);
            builder.Property(l => l.Password).IsRequired();
        }
    }
}
