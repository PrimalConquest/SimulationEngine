using DBWrapper.Source.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBWrapper.Source.Configs
{
    public class UserLoadoutConfig : IEntityTypeConfiguration<UserLoadout>
    {
        public void Configure(EntityTypeBuilder<UserLoadout> builder)
        {
            builder.ToTable("UserLoadouts");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).UseIdentityColumn();
            builder.Property(l => l.CommanderId).IsRequired();
            builder.Property(l => l.OfficerIdsRaw).HasColumnName("OfficerIds").IsRequired();
            builder.Ignore(l => l.OfficerIds);
        }
    }
}
