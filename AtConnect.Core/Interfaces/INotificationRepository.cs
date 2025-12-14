using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;

namespace AtConnect.Core.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
        Task<PagedResultDto<Notification>> GetByUserPagedAsync(int userId, int page, int pageSize);

    }


}
