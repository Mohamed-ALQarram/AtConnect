using AtConnect.BLL.DTOs;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Interfaces
{
    public interface INotificationService
    {
        Task<ResultDTO<PagedResultDto<NotificationDTO>>> GetNotificationsAsync(int userId, int page, int pageSize);
        Task<ResultDTO<List<NotificationDTO>>> GetUnreadNotificationsAsync(int userId);
        Task<ResultDTO<bool>> MarkAsReadAsync(int id, int userId);
        Task<ResultDTO<bool>> MarkAllAsReadAsync(int userId);
        Task<ResultDTO<bool>> DismissAsync(int id, int userId);
        Task AddNotificationAsync(Notification notification);
    }
}
