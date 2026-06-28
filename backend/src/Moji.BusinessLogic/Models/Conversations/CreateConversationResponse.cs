namespace Moji.BusinessLogic.Models.Conversations;

public record CreateConversationResponse(Guid Id, string Name, bool IsGroup, DateTimeOffset CreatedAt, List<Guid> Members);

