namespace CirMin.Contracts.Models.Messages.SendMessage;

public record SendMessageResponse(long Id, Guid SenderId, Guid ConversationId, string Message, DateTimeOffset SentAt);