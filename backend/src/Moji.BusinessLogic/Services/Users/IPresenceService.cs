namespace Moji.BusinessLogic.Services.Users;

public interface IPresenceService
{
    bool AddConnection(Guid userId, string connectionId);
    bool InvalidateConnection(Guid userId, string connectionId);

    List<string> GetOnlineUserIds();
}