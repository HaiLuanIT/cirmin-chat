namespace CirMin.Contracts.Models.FriendShips.AddFriend;

public record AddFriendRequest(Guid ReceiverId, string Message);
