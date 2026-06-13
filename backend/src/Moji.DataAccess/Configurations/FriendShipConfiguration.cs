using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Configurations;

public class FriendShipConfiguration : IEntityTypeConfiguration<FriendShip>
{
    public void Configure(EntityTypeBuilder<FriendShip> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Status).HasMaxLength(50).HasDefaultValue("Pending");

        builder.Property(x => x.Message).HasMaxLength(250);

        //index help query list of friend requested
        builder.HasIndex(x => new { x.RequesterId, x.Status, x.UpdatedAt })
            .HasDatabaseName("IX_FriendShips_RequesterId_Status_UpdatedAt_Desc").IsDescending(false, false, true);
        
        //index help query list of friend received
        builder.HasIndex(x => new { x.ReceiverId, x.Status, x.UpdatedAt })
            .HasDatabaseName("IX_FriendShips_ReceiverId_Status_UpdatedAt_Desc").IsDescending(false, false, true);
        
        builder.HasOne(x => x.Requester)
            .WithMany(x => x.SentFriendRequests)
            .HasForeignKey(x => x.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Receiver)
            .WithMany(x => x.ReceivedFriendRequests)
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}