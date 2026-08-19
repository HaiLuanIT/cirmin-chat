using CirMin.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CirMin.DataAccess.Configurations;

public class FriendShipConfiguration : IEntityTypeConfiguration<FriendShip>
{
    public void Configure(EntityTypeBuilder<FriendShip> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();

        builder.Property(x => x.Message).HasMaxLength(250);

        builder.HasIndex(x => new { x.UserLeftId, x.UserRightId })
            .IsUnique();
        
        //index help query list of friend requested
        builder.HasIndex(x => new { x.RequesterId, x.Status, x.UpdatedAt })
            .HasDatabaseName("IX_FriendShips_RequesterId_Status_UpdatedAt_Desc").IsDescending(false, false, true);
        
        //index help query list of friend received
        builder.HasIndex(x => new { x.UserRightId, x.Status, x.UpdatedAt })
            .HasDatabaseName("IX_FriendShips_UserRightId_Status_UpdatedAt_Desc").IsDescending(false, false, true);
        
        builder.HasOne(x => x.UserLeft)
            .WithMany()
            .HasForeignKey(x => x.UserLeftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UserRight)
            .WithMany()
            .HasForeignKey(x => x.UserRightId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Requester)
            .WithMany()
            .HasForeignKey(x => x.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}