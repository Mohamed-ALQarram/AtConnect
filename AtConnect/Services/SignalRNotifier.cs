using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.SignalR_Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AtConnect.Services
{
    public class SignalRNotifier : INotifier
    {
        private readonly IHubContext<AtConnectHub> _hubContext;

        public SignalRNotifier(IHubContext<AtConnectHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(int userId, Notification notification)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification);
        }
    }
}
