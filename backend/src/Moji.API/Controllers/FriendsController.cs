using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Models.FriendShips;
using Moji.BusinessLogic.Services.Friends;
using Moji.DataAccess.Commons.Constants;

namespace Moji.API.Controllers;

[Authorize]
public class FriendsController : BaseApiController
{
    private readonly IFriendShipService _friendShipService;

    public FriendsController(IFriendShipService friendShipService)
    {
        _friendShipService = friendShipService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFriends()
    {
        var friendList = await _friendShipService.GetFriendList(CurrentUserId);
        return Ok(friendList);
    }
    
    [HttpGet("requests")]
    public async Task<IActionResult> GetFriendRequests()
    {
        var friendList = await _friendShipService.GetFriendRequestList(CurrentUserId);
        return Ok(friendList);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddFriend([FromBody] FriendRequestModel request)
    {
        await _friendShipService.AddFriend(CurrentUserId, request.ReceiverId, request.Message);
        return StatusCode(201,"Gửi lời mời kết bạn thành công");
    }
    
    [HttpPost("requests/{requestId:Guid}/accept")]
    public async Task<IActionResult> AcceptFriendRequest([FromRoute] Guid requestId)
    {
        await _friendShipService.ResponseFriendRequest(CurrentUserId, requestId, FriendShipStatus.Accept);
        return Ok(new { message = "Đã chấp nhận lời mời kết bạn!" });
    }
    
    [HttpPost("requests/{requestId:Guid}/reject")]
    public async Task<IActionResult> RejectFriendRequest([FromRoute] Guid requestId)
    {
        await _friendShipService.ResponseFriendRequest(CurrentUserId, requestId, FriendShipStatus.Reject);
        return Ok(new { message = "Đã từ chối lời mời kết bạn!" });
    }
}