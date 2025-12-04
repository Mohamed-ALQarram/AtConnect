using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AtConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ApiControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDTO<List<NotificationDTO>>>> GetNotifications([FromQuery] PaginationRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ResultDTO<List<NotificationDTO>>(false, "Invalid or missing user ID in token"));

            var response = await _notificationService.GetNotificationsAsync(userId.Value, request.Page, request.PageSize);
            if (!response.Success)
                return BadRequest(response);
            return response;
        }

        [HttpGet("unread")]
        public async Task<ActionResult<ResultDTO<List<NotificationDTO>>>> GetUnread()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ResultDTO<List<NotificationDTO>>(false, "Invalid or missing user ID in token"));

            var response = await _notificationService.GetUnreadNotificationsAsync(userId.Value);
            if (!response.Success)
                return BadRequest(response);
            return response;
        }

        [HttpPost("read-all")]
        public async Task<ActionResult<ResultDTO<bool>>> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ResultDTO<bool>(false, "Invalid or missing user ID in token"));

            var response = await _notificationService.MarkAllAsReadAsync(userId.Value);
            if (!response.Success)
                return BadRequest(response);
            return response;
        }

    }
}
