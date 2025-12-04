using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
        Task<List<Notification>> GetByUserPagedAsync(int userId, int page, int pageSize);

    }


}
