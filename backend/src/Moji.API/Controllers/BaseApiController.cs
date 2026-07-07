using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Moji.BusinessLogic.Exceptions;

namespace Moji.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new MojiUnauthorizedException("Phiên làm việc không hợp lệ hoặc đã hết hạn!");
            }
            return Guid.Parse(userIdClaim.Value);
        }
    }
}