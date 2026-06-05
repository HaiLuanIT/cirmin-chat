using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moji.BusinessLogic.Models.Auth;
using Moji.BusinessLogic.Services.Auth;

namespace Moji.API.Controllers;

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
       Response.Cookies.Append("rt", result.RefreshToken, new CookieOptions
       {
           HttpOnly = true,
           Secure = true,
           SameSite = SameSiteMode.None,
           Expires = DateTimeOffset.Now.AddDays(7)
       });

       return Ok(new { User = result.User, AccessToken = result.AccessToken });
   }
   
   [HttpPost("signup")]
   public async Task<IActionResult> SignUp([FromBody] RegisterRequest registerRequest)
   {
       await _authService.SignUp(registerRequest);
       return Ok();
   }
   
   [HttpGet("test")]
   [Authorize]
   public async Task<IActionResult> Test()
   {
       return Ok(new { Message = "Test" });
   }
}