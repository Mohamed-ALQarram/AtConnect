using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AtConnect.Controllers
{
    /// <summary>
    /// Base controller providing common functionality for all API controllers
    /// </summary>
    public abstract class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Extracts and validates the current user ID from JWT claims
        /// </summary>
        /// <returns>The authenticated user's ID, or null if invalid/missing</returns>
        protected int? GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }
            return userId;
        }
    }
}
