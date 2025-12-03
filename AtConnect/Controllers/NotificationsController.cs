using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AtConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDTO<List<NotificationDTO>>>> GetNotifications([FromQuery] PaginationRequest request)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int userId);
            var response = await _notificationService.GetNotificationsAsync(userId, request.Page, request.PageSize);
            if (!response.Success)
                return Unauthorized(response);
            return response;
        }

        [HttpGet("unread")]
        public async Task<ActionResult<ResultDTO<List<NotificationDTO>>>> GetUnread()
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int userId);
            var response = await _notificationService.GetUnreadNotificationsAsync(userId);
            if (!response.Success)
                return Unauthorized(response);
            return response;
        }

        [HttpPost("read-all")]
        public async Task<ActionResult<ResultDTO<bool>>> MarkAllAsRead()
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int userId);
            var response = await _notificationService.MarkAllAsReadAsync(userId);
            if (!response.Success)
                return BadRequest(response);
            return response;
        }


    }
}
