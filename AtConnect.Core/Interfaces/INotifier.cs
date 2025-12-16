using AtConnect.Core.Models;
using System.Threading.Tasks;

namespace AtConnect.Core.Interfaces
{
    public interface INotifier
    {
        Task SendNotificationAsync(int userId, Notification notification);
    }
}
