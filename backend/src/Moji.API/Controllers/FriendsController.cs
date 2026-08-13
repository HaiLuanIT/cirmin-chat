using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moji.BusinessLogic.Services.Friends;
using Moji.Contracts.Models.FriendShips.AddFriend;

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

    [HttpPost("requests")]
    public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest request, CancellationToken cancellationToken)
    {
        await _friendShipService.AddFriend(CurrentUserId, request, cancellationToken);
        return StatusCode(201, "Gửi lời mời kết bạn thành công");
    }

    [HttpPost("requests/{requestId:Guid}/accept")]
    public async Task<IActionResult> AcceptFriendRequest([FromRoute] Guid requestId)
    {
        await _friendShipService.ProcessFriendRequest(CurrentUserId, requestId, true);
        return Ok(new { message = "Đã chấp nhận lời mời kết bạn!" });
    }

    [HttpPost("requests/{requestId:Guid}/reject")]
    public async Task<IActionResult> RejectFriendRequest([FromRoute] Guid requestId)
    {
        await _friendShipService.ProcessFriendRequest(CurrentUserId, requestId, false);
        return Ok(new { message = "Đã từ chối lời mời kết bạn!" });
    }
}