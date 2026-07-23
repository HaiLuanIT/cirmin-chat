namespace Moji.Contracts.Models.FriendShips;

public record FriendRequestResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public string Username { get; init; }
    public string DisplayName { get; init; }
    public string AvatarUrl { get; init; }
    public string Status { get; init; }
}

public record FriendRequestListResponse
{
    public List<FriendRequestResponse> Inbound { get; init; }
    public List<FriendRequestResponse> Outbound { get; init; }
}