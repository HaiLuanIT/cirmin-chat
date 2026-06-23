namespace Moji.BusinessLogic.Models;

public record SendMessageGroupResponse(Guid MessageId, Guid SenderId, Guid ConversationId, string Message, DateTimeOffset SentAt);