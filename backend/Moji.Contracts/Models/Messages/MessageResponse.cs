namespace Moji.Contracts.Models.Messages;

public record MessageResponse
{
    public long Id { get; init; }

    public string Content { get; init; }

    public Guid ConversationId { get; init; }

    public SenderResponse Sender { get; init; }

    public DateTimeOffset SentAt { get; init; }
}

public record SenderResponse
{
    public Guid SenderId { get; init; }

    public string DisplayName { get; init; }

    public string AvatarUrl { get; init; }
}