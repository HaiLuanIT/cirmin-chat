namespace Moji.DataAccess.Repositories.Models;

public record FriendshipRawData(Guid FriendId, string fullName, string avatarUrl, string status, Guid RequestId);