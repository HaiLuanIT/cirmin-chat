using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using CirMin.BusinessLogic.Exceptions;

namespace CirMin.API.Controllers;

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
                throw new CirMinUnauthorizedException("Phiên làm việc không hợp lệ hoặc đã hết hạn!");
            }
            return Guid.Parse(userIdClaim.Value);
        }
    }
}