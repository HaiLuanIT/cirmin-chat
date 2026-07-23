namespace Moji.Contracts.Models.FriendShips;

public record FriendResponse
{
    public Guid UserId { get; init; }

    public string Username { get; init; }
    public string DisplayName { get; init; }
    public string AvatarUrl { get; init; }
    public string Status { get; init; }
}