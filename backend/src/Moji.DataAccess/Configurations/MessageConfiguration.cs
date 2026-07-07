using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).UseIdentityByDefaultColumn();

        builder.Property(x => x.Content).IsRequired();

        builder.Property(x => x.ImageUrl).HasMaxLength(500);

        //use composite inex to group message by conversation and sort from newest to oldest
        //add id to support filter message duplicate time
        builder.HasIndex(x => new { x.ConversationId, x.CreatedAt, x.Id })
            .HasDatabaseName("IX_Messages_ConversationId_CreatedAt_Id").IsDescending(false, true, true);
        
        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Sender)
            .WithMany()
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}