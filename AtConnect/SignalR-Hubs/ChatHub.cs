using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace AtConnect.SignalR_Hubs
{
    public class AtConnectHub:Hub
    {
        private readonly IUserConnectionManager _userConnectionManager;

        public AtConnectHub(IUserConnectionManager userConnectionManager)
        {
            _userConnectionManager = userConnectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            int.TryParse(Context.UserIdentifier, out int userId);
            if (userId >0)
                _userConnectionManager.AddConnection(userId, Context.ConnectionId);
            
            await base.OnConnectedAsync();
            
        }
        public async Task SendMessage(Message message)
        {
            
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int.TryParse(Context.UserIdentifier, out int userId);
            if (userId > 0)
                _userConnectionManager.RemoveConnection(userId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
