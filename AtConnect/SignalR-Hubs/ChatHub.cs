using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Services;
using AtConnect.Core.Enum;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DTOs.HubDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MimeKit;

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
            if (userId < 1) throw new HubException("Could not access user Id from token");
            return userId;
        }
        public override async Task OnConnectedAsync()
        {
            int userId = getUserId();
                _userConnectionManager.AddConnection(userId, Context.ConnectionId);
            
            await base.OnConnectedAsync();
            
        }
        public async Task JoinChat(int chatId)
        {
            if(! await chatService.IsChatParticipantAsync(chatId, getUserId()))
                throw new HubException("Not allowed to join this chat.");

            int readerId = getUserId();

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            bool anyUpdated = await chatService.MarkChatMessagesAsReadAsync(chatId, readerId);
            if(anyUpdated)
                await Clients.Group(chatId.ToString()).SendAsync("MessagesSeen", new
                {
                    ChatId = chatId,
                    ReaderId = readerId,
                    ReadAt = DateTime.UtcNow
                });
        }
        public async Task SendMessage(SendMessageRequest msgRequest)
        {
            int userId = getUserId();
            
            // Get the receiver from the database (also validates sender is a participant)
            int? receiverId = await chatService.GetOtherParticipantIdAsync(msgRequest.ChatId, userId);
            
            if (receiverId == null)
                throw new HubException("Not allowed to send messages in this chat.");

            var message = new Message(userId, msgRequest.ChatId, msgRequest.Content);
            await chatService.SaveChatMessage(message);

            await Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", 
                new { MessageId = message.Id, message.SenderId, message.ChatId, message.Content, message.SentAt });

            var notification = new Notification(receiverId.Value, msgRequest.ChatId, null, message.Content, NotificationType.NewMessage);
            await notificationService.AddNotificationAsync(notification);
            await Clients.User(receiverId.Value.ToString()).SendAsync("ReceiveNotification", notification);
        }
        public async Task MarkMessagesAsRead(int chatId)
        {
            int readerId = getUserId();

            bool anyUpdated = await chatService.MarkChatMessagesAsReadAsync(chatId, readerId);

            if (anyUpdated)
            {
                await Clients.Group(chatId.ToString()).SendAsync("MessagesSeen", new
                {
                    ChatId = chatId,
                    ReaderId = readerId,
                    ReadAt = DateTime.UtcNow
                });
            }
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
