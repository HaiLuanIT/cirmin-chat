namespace Moji.Contracts.Models.Users.SearchUser;

public class SearchUserResponse
{
    public Guid Id { get; init; }
    
    public string Username { get; init; }
    
    public string DisplayName { get; init; }
    
    public string? AvatarUrl { get; init; }
    
    public string RelationStatus { get; init; }
    
    public Guid? ConversationId { get; init; }
}