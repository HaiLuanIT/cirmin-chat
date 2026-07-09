namespace Moji.Contracts.Models.Conversations;

public record ConversationModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsGroup { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public LastMessageModel? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public List<ConversationMemberModel> Members { get; set; }
}

public record LastMessageModel
{
    public long? Id { get; set; }
    public string? LastMessageContent { get; set; }
    public DateTimeOffset? LastMessageAt { get; set; }
}

public record ConversationMemberModel
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
}