namespace Moji.BusinessLogic.Models.FriendShips;

public record FriendResponse(Guid UserId, string FullName, string AvatarUrl, string Status);