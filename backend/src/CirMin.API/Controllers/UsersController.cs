using CirMin.API.Helpers;
using CirMin.API.Models.Users.UploadAvatar;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CirMin.BusinessLogic.Exceptions;
using CirMin.BusinessLogic.Helpers;
using CirMin.BusinessLogic.Services.Auth;
using CirMin.BusinessLogic.Services.Users;
using CirMin.Contracts.Models.Auth.ChangePassword;
using CirMin.Contracts.Models.Users.SearchUser;
using CirMin.Contracts.Models.Users.UpdateUserInfo;

namespace CirMin.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly IValidator<UpdateAvatarHttpRequest> _updateAvatarValidator;
    private readonly IUserService _userService;

    public UsersController(IUserService userService, IValidator<UpdateAvatarHttpRequest> updateAvatarValidator,
        IAuthService authService)
    {
        _userService = userService;
        _updateAvatarValidator = updateAvatarValidator;
        _authService = authService;
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
            throw new CirMinValidationException(validationResult.Errors);

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

    [HttpPatch("me/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _authService.ChangePassword(CurrentUserId, request, cancellationToken);
        Response.Cookies.Delete("rt", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });
        return NoContent();
    }
}