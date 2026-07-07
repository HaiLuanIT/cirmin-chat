namespace Moji.BusinessLogic.Models.FriendShips;

public record FriendResponse(Guid UserId, string DisplayName, string AvatarUrl, string Status);