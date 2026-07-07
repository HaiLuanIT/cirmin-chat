using System.Collections.Concurrent;

namespace Moji.BusinessLogic.Services.Users;

public class PresenceService : IPresenceService
{
    private static readonly ConcurrentDictionary<Guid, HashSet<string>> OnlineUsers = new();
    public bool AddConnection(Guid userId, string connectionId)
    {
        bool isFirstConnection = false;
        OnlineUsers.AddOrUpdate(userId, _ =>
        {
            isFirstConnection = true;
            return new HashSet<string> { connectionId };
        }, (_, connections) =>
        {
            lock (connections)
            {
                
                connections.Add(connectionId);
            }
            return connections;
            
        });
        return isFirstConnection;
    
    }

    public bool InvalidateConnection(Guid userId, string connectionId)
    {
        bool isLastConnection = false;

        if (OnlineUsers.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    isLastConnection = true;
                }
            }

            if (isLastConnection)
            {
                OnlineUsers.TryRemove(KeyValuePair.Create(userId, connections));
            }
        }

        return isLastConnection;
    }
    
    public List<string> GetOnlineUserIds()
    {
        return OnlineUsers
            .Select(pair => pair.Key.ToString())
            .ToList();
    }

}