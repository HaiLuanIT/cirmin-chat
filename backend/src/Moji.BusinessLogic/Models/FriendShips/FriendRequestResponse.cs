namespace Moji.BusinessLogic.Models.FriendShips;

public record FriendRequestResponse(Guid RequestId, Guid UserId, string FullName, string AvatarUrl, string Status);

public record FriendRequestListResponse(
    List<FriendRequestResponse> Inbound,
    List<FriendRequestResponse> Outbound
);