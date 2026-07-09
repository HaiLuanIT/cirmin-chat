namespace Moji.Contracts.Models.Messages;

public record GetConversationMessagesRequest
{
    public string? NextCursor { get; init; }

    public int Limit { get; set; } = 20;
}