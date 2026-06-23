namespace Moji.BusinessLogic.Models;

public record SendMessageRequest(Guid ReceiverId, string Content, Guid ConversationId);