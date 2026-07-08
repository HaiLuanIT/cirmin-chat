using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Moji.BusinessLogic.Services.Conversations;
using Moji.BusinessLogic.Services.Friends;
using Moji.BusinessLogic.Services.Users;

namespace Moji.API.Hubs;

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly IConversationService _conversationService;
    private readonly IPresenceService _presenceService;
    private readonly IFriendShipService _friendShipService;

    public ChatHub(IConversationService conversationService, IPresenceService presenceService, IFriendShipService friendShipService)
    {
        _conversationService = conversationService;
        _presenceService = presenceService;
        _friendShipService = friendShipService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        
        var isFirstConnection = _presenceService.AddConnection(userId, Context.ConnectionId);

        var currentUsersOnline = _presenceService.GetOnlineUserIds();
        
        await Clients.Caller.GetOnlineUsers(currentUsersOnline);

        if (isFirstConnection)
        {
            var friendIds = await _friendShipService.GetFriendIds(userId);
            await Clients.Users(friendIds).UserStatusChanged(userId.ToString(), true);
        }
        
        //add user to group conversation
        var conversationIds = await _conversationService.GetJoinedConversationId(userId);
        foreach (var conversationId in conversationIds)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        var isLastConnection = _presenceService.InvalidateConnection(userId, Context.ConnectionId);

        if (isLastConnection)
        {
            var friendIds = await _friendShipService.GetFriendIds(userId);
            await Clients.Users(friendIds).UserStatusChanged(userId.ToString(), false);
        }
        //remove user to all group conversation
        var conversationIds = await _conversationService.GetJoinedConversationId(userId);
        foreach (var conversationId in conversationIds)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }
        await base.OnDisconnectedAsync(exception);
    }


    private Guid GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new HubException("Xác thực danh tính thất bại.");
        }

        return userId;
    }
}