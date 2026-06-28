namespace Moji.BusinessLogic.Models.Conversations;

public record ConversationModel(
    Guid Id,
    string Name,
    bool IsGroup,
    DateTimeOffset CreatedAt,
    LastMessageModel? LastMessage,
    int UnreadCount,
    List<ConversationMemberModel> Members);


public record LastMessageModel(long? Id, string? LastMessageContent, DateTimeOffset? LastMessageAt);

public record ConversationMemberModel(Guid UserId, string DisplayName, string? AvatarUrl);