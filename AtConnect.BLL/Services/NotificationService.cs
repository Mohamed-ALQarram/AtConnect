using AtConnect.BLL.DTOs;
using AtConnect.Core.SharedDTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtConnect.Core.Models;


namespace AtConnect.BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _uow;

        public NotificationService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<ResultDTO<PagedResultDto<NotificationDTO>>> GetNotificationsAsync(int userId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            var notificationsQuery = await _uow.Notifications.GetByUserPagedAsync(userId, page, pageSize);

            return new ResultDTO<PagedResultDto<NotificationDTO>> (true,"notification sent",notificationsQuery);

          
        }

        public async Task<ResultDTO<List<NotificationDTO>>> GetUnreadNotificationsAsync(int userId)
        {
            var notificationsQuery = (List<NotificationDTO>)await _uow.Notifications.GetUnreadNotificationsAsync(userId);
           

            return new ResultDTO<List<NotificationDTO>>(true, "Unread notifications retrieved successfully", notificationsQuery);
        }

        public async Task<ResultDTO<bool>> MarkAllAsReadAsync(int userId)
        {
            int updatedCount = await _uow.Notifications.MarkAllAsReadAsync(userId);
            if (updatedCount == 0)
            {
                return new ResultDTO<bool>(false, "No unread notifications found to mark as read");
            }

            return new ResultDTO<bool>(true, "All notifications marked as read", true);
        }

        public async Task<ResultDTO<bool>> MarkAsReadAsync(int id, int userId)
        {
            var notification = await _uow.Notifications.GetByKeysAsync(id);
            if (notification == null || notification.ReceiverId != userId)
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
            if (notification == null || notification.ReceiverId != userId)
            {
                return new ResultDTO<bool>(false, "Notification not found or access denied");
            }

            _uow.Notifications.Delete(notification);
            await _uow.SaveChangesAsync();

            return new ResultDTO<bool>(true, "Notification dismissed", true);
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _uow.Notifications.AddAsync(notification);
            await _uow.SaveChangesAsync();
        }
    }
}
