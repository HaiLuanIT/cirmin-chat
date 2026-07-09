namespace Moji.Contracts.Models.Conversations.CreateConversation;

public record CreateConversationResponse(Guid Id, string Name, bool IsGroup, DateTimeOffset CreatedAt, List<Guid> Members);

