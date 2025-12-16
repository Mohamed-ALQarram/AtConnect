using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Services;
using AtConnect.Core.Enum;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DTOs.HubDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AtConnect.SignalR_Hubs
{
    [Authorize]
    public class AtConnectHub:Hub
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IChatService chatService;
        private readonly INotificationService notificationService;
        public AtConnectHub(IUserConnectionManager userConnectionManager, IChatService chatService, INotificationService notificationService)
        {
            _userConnectionManager = userConnectionManager;
            this.chatService = chatService;
            this.notificationService = notificationService;
        }
        private int getUserId()
        {
            int.TryParse(Context.UserIdentifier, out int userId);
            return userId;
        }
        public override async Task OnConnectedAsync()
        {
            int userId = getUserId();
            if (userId >0)
                _userConnectionManager.AddConnection(userId, Context.ConnectionId);
            
            await base.OnConnectedAsync();
            
        }
        public async Task JoinChat(int chatId)
        {
            if(! await chatService.IsChatParticipantAsync(chatId, getUserId()))
                throw new HubException("Not allowed to join this chat.");
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }
        public async Task SendMessage(SendMessageRequest msgRequest)
        {
            int userId = getUserId();
            var message = new Message(userId, msgRequest.ChatId, msgRequest.Content);
            await chatService.SaveChatMessage(message);

            await Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", 
                new{MessageId= message.Id, message.SenderId, message.ChatId, message.Content, message.SentAt});

            var notification = new Notification(msgRequest.ReceiverId, msgRequest.ChatId,null, message.Content ,NotificationType.NewMessage);
            await notificationService.AddNotificationAsync(notification);
            await Clients.User(msgRequest.ReceiverId.ToString()).SendAsync("ReceiveNotification", notification);
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
