using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SourceName.Data.Model.Role;

namespace SourceName.Data.Model.EntityTypeConfigurations.Roles
{
    public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
    {
        public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.ApplicationUserId)
                .IsRequired();

            builder.Property(x => x.ApplicationRoleId)
                .IsRequired();

            builder.HasIndex(x => new { x.ApplicationUserId, x.ApplicationRoleId })
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.ApplicationUserId);

            builder.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.ApplicationRoleId);
        }
    }
}