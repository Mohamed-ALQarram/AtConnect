using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
using System.Threading.Tasks;

namespace AtConnect.Core.Interfaces
{
    public interface INotifier
    {
        Task SendNotificationAsync(int userId, NotificationDTO notification);
    }
}
