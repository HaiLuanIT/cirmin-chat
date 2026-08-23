using CirMin.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CirMin.DataAccess.Configurations;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Token).HasMaxLength(500).IsRequired();

        builder.Property(x => x.ExpiresAt);

        builder.Property(x => x.IsRevoked).HasDefaultValue(false);

        builder.Property(x => x.AuthVersion).HasDefaultValue(0).IsRequired();

        builder.HasIndex(x => x.Token).IsUnique();

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}