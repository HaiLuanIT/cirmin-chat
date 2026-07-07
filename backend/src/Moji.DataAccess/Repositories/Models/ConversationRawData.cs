namespace Moji.DataAccess.Repositories.Models;

public record ConversationRawData
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public bool IsGroup { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public LastMessageRawData? LastMessage { get; init; }
    public int UnreadCount { get; init; }
    public IEnumerable<ConversationMemberRawData> Members { get; init; }
}

public record LastMessageRawData
{
    public long? Id { get; init; }
    public string? LastMessage { get; init; }
    public DateTimeOffset? LastMessageAt { get; init; }
}

public record ConversationMemberRawData
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
}