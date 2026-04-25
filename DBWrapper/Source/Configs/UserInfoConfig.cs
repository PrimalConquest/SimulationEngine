using DBWrapper.Source.Models;
using Microsoft.EntityFrameworkCore;

namespace DBWrapper.Source.Configs
{
    public class UserInfoConfig : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserInfo> builder)
        {
            builder.ToTable("UserInfos");

            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).UseIdentityColumn();

            builder.Property(l => l.CommanderId).IsRequired();
            builder.Property(l => l.OfficerIdsRaw).HasColumnName("OfficerIds").IsRequired();
            builder.Property(l => l.RankPoints).IsRequired();

            // Ignore the computed property
            builder.Ignore(l => l.OfficerIds);

            //Reference to User so user info references user
            builder.HasOne(l => l.User).WithOne(o => o.Info).HasForeignKey<UserInfo>(l => l.UserId).HasConstraintName("FK_UserInfo_To_User");
        }
    }
}
