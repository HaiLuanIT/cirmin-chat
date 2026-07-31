using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moji.API.Models.Users.UploadAvatar;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Helpers;
using Moji.BusinessLogic.Services.Users;
using Moji.Contracts.Models.Users.SearchUser;
using Moji.Contracts.Models.Users.UpdateUserInfo;

namespace Moji.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IValidator<UpdateAvatarHttpRequest> _updateAvatarValidator;
    private readonly IUserService _userService;

    public UsersController(IUserService userService, IValidator<UpdateAvatarHttpRequest> updateAvatarValidator)
    {
        _userService = userService;
        _updateAvatarValidator = updateAvatarValidator;
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

    [HttpPut("me/avatar")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(1024 * 1024 * 6)]
    public async Task<IActionResult> UploadAvatar([FromForm] UpdateAvatarHttpRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _updateAvatarValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new MojiValidationException(validationResult.Errors);

        await ImageValidatorHelper.IsSupportedImageAsync(request.Image!);

        var fileName = request.Image!.FileName;
        using var stream = request.Image!.OpenReadStream();

        var result = await _userService.UpdateUserAvatarAsync(CurrentUserId, stream, fileName, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("me/update-information")]
    public async Task<IActionResult> UpdataUserInformation([FromBody] UpdateUserInfoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateUserInfoAsync(CurrentUserId, request, cancellationToken);
        return Ok(result);
    }
}