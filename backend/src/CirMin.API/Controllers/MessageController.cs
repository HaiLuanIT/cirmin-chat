using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CirMin.API.Hubs;
using CirMin.BusinessLogic.Services.Conversations;
using CirMin.BusinessLogic.Services.Messages;
using CirMin.Contracts.Models.Messages.SendMessage;

namespace CirMin.API.Controllers;

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