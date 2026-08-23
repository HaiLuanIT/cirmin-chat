namespace CirMin.Contracts.Models.Messages.SendMessage;

public record SendMessageRequest(string Content, Guid ConversationId, string? ImgUrl);