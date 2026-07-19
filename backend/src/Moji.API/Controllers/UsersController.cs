using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moji.BusinessLogic.Services.Users;
using Moji.Contracts.Models.Users.SearchUser;

namespace Moji.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchUserByUsername([FromQuery] string username, [FromQuery] int limit = 10,
        [FromQuery] int pageNumber = 1)
    {
        var requestModel = new SearchUserRequest
        {
            Username = username,
            PageNumber = pageNumber,
            PageSize = limit
        };
        var result = await _userService.SearchByUsername(CurrentUserId, requestModel);
        return Ok(result);
    }
}