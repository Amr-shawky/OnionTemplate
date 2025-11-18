using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OnionTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        protected string GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        protected string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }

        protected bool IsAdmin()
        {
            return GetCurrentUserRole() == "Admin";
        }

        protected bool IsManager()
        {
            return GetCurrentUserRole() == "Manager" || IsAdmin();
        }
    }
}

