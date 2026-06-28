using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moji.BusinessLogic.Models.Conversations;
using Moji.BusinessLogic.Services.Conversations;

namespace Moji.API.Controllers;

[Authorize]
public class ConversationsController : BaseApiController
{
    private readonly IConversationService _conversationService;

    public ConversationsController(IConversationService conversationService)
    {
        _conversationService = conversationService;
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
    
}