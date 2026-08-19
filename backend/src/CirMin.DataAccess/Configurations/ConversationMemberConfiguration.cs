using CirMin.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CirMin.DataAccess.Configurations;

public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        builder.HasKey(cm => new { cm.ConversationId, cm.UserId });
        
        builder.Property(cm => cm.UnreadCount).HasDefaultValue(0);

        builder.Property(cm => cm.LastSeenMessageId).HasDefaultValue(0);

        builder.Property(cm => cm.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.HasOne(cm => cm.Conversation)
            .WithMany(cm => cm.Members)
            .HasForeignKey(cm => cm.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(cm => cm.User)
            .WithMany(cm => cm.Conversations)
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}