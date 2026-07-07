namespace Moji.BusinessLogic.Models.Conversations;

public record CreateConversationRequest(List<Guid> UserIds, string Name);