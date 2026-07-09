using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moji.BusinessLogic.Services.Conversations;
using Moji.BusinessLogic.Services.Messages;
using Moji.Contracts.Models.Conversations.CreateConversation;

namespace Moji.API.Controllers;

[Authorize]
public class ConversationsController : BaseApiController
{
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;

    public ConversationsController(IConversationService conversationService, IMessageService messageService)
    {
        _conversationService = conversationService;
        _messageService = messageService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateConversationRequest request)
    {
        var res = await _conversationService.CreateConversation(CurrentUserId, request);
        return StatusCode(201, res);
    }
       
    [HttpGet]
    public async Task<IActionResult> GetConversations()
    {
        var conversations = await _conversationService.GetConversations(CurrentUserId);
        return Ok(conversations);
    }
    
    [HttpGet("{id:guid}/messages")]
    public async Task<IActionResult> GetConversationMessages(
        [FromRoute] Guid id,
        [FromQuery] int limit = 20,
        [FromQuery] string cursor = null)

    {
        var result = await _messageService.GetConversationMessages(CurrentUserId, id, limit, cursor);
        return Ok(result);
    }

    [HttpPost("{id:guid}/mark-as-seen")]
    public async Task<IActionResult> MarkAsSeen([FromRoute] Guid id)
    {
        await _messageService.MarkAsSeen(CurrentUserId, id);
        return Ok();
    }
}