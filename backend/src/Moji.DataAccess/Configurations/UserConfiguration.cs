using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Username).HasMaxLength(50).IsRequired();

        builder.Property(x => x.HashedPassword).IsRequired();

        builder.Property(x => x.Email).HasMaxLength(50);

        builder.Property(x => x.FullName).HasMaxLength(100).IsRequired();

        builder.Property(x => x.AvatarId).HasMaxLength(500);

        builder.Property(x => x.AvatarUrl).HasMaxLength(1028);

        builder.Property(x => x.Bio).HasMaxLength(500);

        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex(x => x.Username).IsUnique()
            .HasDatabaseName("IX_Users_Username_Unique");

        builder.HasIndex(x => x.Email).IsUnique()
            .HasDatabaseName("IX_Users_Email_Unique");
    }
}