using AtConnect.BLL.DTOs;
using AtConnect.Core.SharedDTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AtConnect.BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _uow;

        public NotificationService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<ResultDTO<List<NotificationDTO>>> GetNotificationsAsync(int userId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            // Use repository method (single source of truth for paging)
            var entities = await _uow.Notifications.GetByUserPagedAsync(userId, page, pageSize);

            // Map to DTO
            var items = entities.Select(n => new NotificationDTO
            {
                Id = n.Id,
                Type = n.Type.ToString(),
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();

            return new ResultDTO<List<NotificationDTO>>(true, "Notifications retrieved successfully", items);
        }

        public async Task<ResultDTO<List<NotificationDTO>>> GetUnreadNotificationsAsync(int userId)
        {
            var entities = await _uow.Notifications.GetUnreadNotificationsAsync(userId);
            var items = entities.Select(n => new NotificationDTO
            {
                Id = n.Id,
                Type = n.Type.ToString(),
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();

            return new ResultDTO<List<NotificationDTO>>(true, "Unread notifications retrieved successfully", items);
        }

        public async Task<ResultDTO<bool>> MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _uow.Notifications.GetUnreadNotificationsAsync(userId);
            if (!unreadNotifications.Any())
            {
                return new ResultDTO<bool>(true, "No unread notifications to mark", true);
            }

            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
                _uow.Notifications.Update(notification);
            }
            await _uow.SaveChangesAsync();

            return new ResultDTO<bool>(true, "All notifications marked as read", true);
        }

        public async Task<ResultDTO<bool>> MarkAsReadAsync(int id, int userId)
        {
            var notification = await _uow.Notifications.GetByKeysAsync(id);
            if (notification == null || notification.UserId != userId)
            {
                return new ResultDTO<bool>(false, "Notification not found or access denied");
            }

            notification.MarkAsRead();
            _uow.Notifications.Update(notification);
            await _uow.SaveChangesAsync();

            return new ResultDTO<bool>(true, "Notification marked as read", true);
        }

        public async Task<ResultDTO<bool>> DismissAsync(int id, int userId)
        {
            var notification = await _uow.Notifications.GetByKeysAsync(id);
            if (notification == null || notification.UserId != userId)
            {
                return new ResultDTO<bool>(false, "Notification not found or access denied");
            }

            _uow.Notifications.Delete(notification);
            await _uow.SaveChangesAsync();

            return new ResultDTO<bool>(true, "Notification dismissed", true);
        }

       

    }
}
