namespace Moji.BusinessLogic.Models.FriendShips;

public record FriendRequestResponse(Guid Id, Guid UserId, string DisplayName, string AvatarUrl, string Status);

public record FriendRequestListResponse(
    List<FriendRequestResponse> Inbound,
    List<FriendRequestResponse> Outbound
);