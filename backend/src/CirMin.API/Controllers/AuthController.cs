using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CirMin.BusinessLogic.Services.Auth;
using CirMin.Contracts.Models.Auth;
using CirMin.Contracts.Models.Auth.Login;
using CirMin.Contracts.Models.Auth.Register;

namespace CirMin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] LoginRequest loginRequest)
    {
        var result = await _authService.SignIn(loginRequest);
        AppendRefreshTokenCookie(result);

        return Ok(new { User = result.User, AccessToken = result.AccessToken });
    }

    private void AppendRefreshTokenCookie(AuthResponse result)
    {
        Response.Cookies.Append("rt", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.Now.AddDays(7)
        });
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] RegisterRequest registerRequest)
    {
        await _authService.SignUp(registerRequest);
        return Ok();
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SingOut()
    {
        if (!Request.Cookies.TryGetValue("rt", out var refreshToken))
        {
            return Ok(new { Message = "Đăng xuất thành công!" });
        }

        await _authService.RevokeRefreshToken(refreshToken);
        Response.Cookies.Delete("rt", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });
        return Ok(new { Message = "Đăng xuất thành công!" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> FetchMe(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        Guid currentUserId = Guid.Parse(userId.Value);
        var user = await _authService.GetUser(currentUserId, cancellationToken);
        return Ok(user);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("rt", out var oldToken))
        {
            return Unauthorized(new { Message = "Phiên đăng nhập đã hết hạn!" });
        }
        var result = await _authService.RefreshToken(oldToken, cancellationToken);
        
        AppendRefreshTokenCookie(result);
        return Ok(new { User = result.User, AccessToken = result.AccessToken });

    }
    [Authorize]
    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        return Ok("Test");
    }
}