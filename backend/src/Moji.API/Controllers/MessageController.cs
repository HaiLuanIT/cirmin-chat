using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Services.Messages;

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
        var result = await _messageService.SendMessage(CurrentUserId, request);
        return Ok(result);
    }
}