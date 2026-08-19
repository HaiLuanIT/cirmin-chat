using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using CirMin.BusinessLogic.Services.Conversations;
using CirMin.BusinessLogic.Services.Friends;
using CirMin.BusinessLogic.Services.Messages;
using CirMin.BusinessLogic.Services.Users;

namespace CirMin.API.Hubs;

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly IConversationService _conversationService;
    private readonly IFriendShipService _friendShipService;
    private readonly IMessageService _messageService;
    private readonly IPresenceService _presenceService;

    public ChatHub(IConversationService conversationService, IPresenceService presenceService,
        IFriendShipService friendShipService, IMessageService messageService)
    {
        _conversationService = conversationService;
        _presenceService = presenceService;
        _friendShipService = friendShipService;
        _messageService = messageService;
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
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);

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
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task MarkConversationAsRead(string conversationId)
    {
        var userId = GetUserId();

        var lastMessageId = await _messageService.MarkAsSeen(userId, Guid.Parse(conversationId));

        if (lastMessageId != null)
            await Clients.Group(conversationId)
                .UserSeenMessage(userId.ToString(), conversationId, lastMessageId.ToString());
    }

    public async Task JoinConversation(string conversationId)
    {
        var userId = GetUserId();
        var isMember = await _conversationService.IsMember(userId, Guid.Parse(conversationId));
        if (!isMember) throw new HubException("Bạn không thuộc về cuộc trò chuyện này!");

        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }


    private Guid GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId)) throw new HubException("Xác thực danh tính thất bại.");

        return userId;
    }
}