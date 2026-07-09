namespace Moji.Contracts.Models.Conversations.CreateConversation;

public record CreateConversationRequest(List<Guid> UserIds, string Name);