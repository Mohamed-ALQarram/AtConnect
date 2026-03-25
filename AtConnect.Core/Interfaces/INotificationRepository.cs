using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;

namespace AtConnect.Core.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<NotificationDTO>> GetUnreadNotificationsAsync(int userId);
       
        Task<PagedResultDto<NotificationDTO>> GetByUserPagedAsync(int userId, int page, int pageSize);

        Task<int> MarkAllAsReadAsync(int userId);

    }


}
