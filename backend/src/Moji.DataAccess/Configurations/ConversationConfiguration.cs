using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.Name).HasMaxLength(100);

        builder.Property(x => x.IsGroup).HasDefaultValue(false).IsRequired();

        builder.Property(x => x.LastMessageId);

        builder.Property(x => x.LastMessage);

        builder.Property(x => x.LastMessageTime);

        builder.HasIndex(x => x.LastMessageTime)
            .HasDatabaseName("IX_Conversations_LastMessageTime_Desc")
            .IsDescending();
    }
}