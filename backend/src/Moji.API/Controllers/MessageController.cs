using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moji.API.Hubs;
using Moji.BusinessLogic.Services.Conversations;
using Moji.BusinessLogic.Services.Messages;
using Moji.Contracts.Models.Messages.SendMessage;

namespace Moji.API.Controllers;

[Authorize]
public class MessageController : BaseApiController
{
    private readonly IMessageService _messageService;
    
    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        await _messageService.SendMessage(CurrentUserId, request);
        return Ok();
    }
    
}