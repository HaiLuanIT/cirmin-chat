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