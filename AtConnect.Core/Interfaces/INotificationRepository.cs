using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
    }


}
