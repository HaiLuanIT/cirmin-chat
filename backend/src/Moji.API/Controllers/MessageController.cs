using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moji.API.Hubs;
using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Services.Conversations;
using Moji.BusinessLogic.Services.Messages;

namespace Moji.API.Controllers;

[Authorize]
public class MessageController : BaseApiController
{
    private readonly IMessageService _messageService;
    private readonly IConversationService _conversationService;
    private readonly IHubContext<ChatHub, IChatClient> _chatHubContext;

    public MessageController(IMessageService messageService, IConversationService conversationService, IHubContext<ChatHub, IChatClient> chatHubContext)
    {
        _messageService = messageService;
        _conversationService = conversationService;
        _chatHubContext = chatHubContext;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var result = await _messageService.SendMessage(CurrentUserId, request);
        
        var memberIds = await _conversationService.GetConversationMemberIds(request.ConversationId);
        if (memberIds.Any())
        {
            await _chatHubContext.Clients.Groups(memberIds)
                .ReceiveMessage(result.ConversationId.ToString(), result.Message);
        }

        return Ok(result);
    }
}