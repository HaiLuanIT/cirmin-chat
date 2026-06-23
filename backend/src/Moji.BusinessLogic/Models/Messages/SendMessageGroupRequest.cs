namespace Moji.BusinessLogic.Models;

public record SendMessageGroupRequest(Guid ConversationId, string Message);