using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Services;
using AtConnect.Core.Enum;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
using AtConnect.DTOs.HubDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IUserService userService;
        public AtConnectHub(IUserConnectionManager userConnectionManager, IChatService chatService, INotificationService notificationService, IUserService userService)
        {
            _userConnectionManager = userConnectionManager;
            this.chatService = chatService;
            this.notificationService = notificationService;
            this.userService = userService;
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
            // Add connection FIRST, then check if this is the first one
            _userConnectionManager.AddConnection(userId, Context.ConnectionId);
            var activeConnections = _userConnectionManager.GetConnections(userId);
            if(activeConnections != null && activeConnections.Count == 1) // This is the FIRST connection
            {
               var user = await userService.GetUserById(userId);
                if(user != null)
                {
                    user.SetActive( true);
                    await userService.UpdateUserAsync(user);//check it
                } 
            }

            await base.OnConnectedAsync();
            
        }
        public async Task SendTyping(int chatId)
        {
            int userId = getUserId();

            if (userId > 0)
            {
                if (!await chatService.IsChatParticipantAsync(chatId, userId))
                    throw new HubException("Not allowed to send typing events in this chat.");

                await Clients.GroupExcept(chatId.ToString(), Context.ConnectionId)
                             .SendAsync("ReceiveTyping", new { userId, chatId });
               
            }
        }
        public async Task SendMessage(SendMessageRequest msgRequest)
        {
            int userId = getUserId();
            
            if (string.IsNullOrWhiteSpace(msgRequest.Content))
                throw new HubException("Message content cannot be empty.");
            
            if (msgRequest.Content.Length > 4000)
                throw new HubException("Message content is too long.");
            
            if (msgRequest.ChatId <= 0)
                throw new HubException("Invalid chat ID.");
            
            var receiverId = await chatService.GetOtherParticipantIdAsync(msgRequest.ChatId, userId);
            var user = await userService.GetUserById(userId);
            
            
            if (receiverId == null)
                throw new HubException("Not allowed to send messages in this chat.");
            
            if (user == null)
                throw new HubException("Sender user not found.");

            var message = new Message(userId, msgRequest.ChatId, msgRequest.Content);
            await chatService.SaveChatMessage(message);

            var msg = new MessageDto(message.Id, message.SenderId, message.ChatId, message.Content, message.SentAt, message.Status);
            await Clients.User(userId.ToString()).SendAsync("ReceiveMessage", msg); 
            await Clients.User(receiverId.Value.ToString()).SendAsync("ReceiveMessage", msg); 

            var notification = new Notification(receiverId.Value, userId, msgRequest.ChatId, null, message.Content, NotificationType.NewMessage);
            
            // Parallel: broadcast message + persist notification + send real-time notification
            var notifyTask = notificationService.AddNotificationAsync(notification);
            var notificationDto = new NotificationDTO
            {
                UserId = userId,
                UserFullName = $"{user.FirstName} {user.LastName}",
                AvatarUrl = user.ImageURL,
                ChatId = msgRequest.ChatId,
                Content = msgRequest.Content,
                CreatedAt = DateTime.UtcNow,
                notificationType = NotificationType.NewMessage,
                IsRead = false,
                RequestId = null
            };
            var realTimeNotifyTask = Clients.User(receiverId.Value.ToString()).SendAsync("ReceiveNotification", notificationDto);
            await Task.WhenAll(notifyTask, realTimeNotifyTask);
        }
        public async Task MarkMessagesAsRead(int chatId)
        {
            int readerId = getUserId();

            if (!await chatService.IsChatParticipantAsync(chatId, readerId))
                throw new HubException("Not allowed to mark messages as read in this chat.");
            var receiverId = await chatService.GetOtherParticipantIdAsync(chatId, readerId);

            bool anyUpdated = await chatService.MarkChatMessagesAsReadAsync(chatId, readerId);

            if (anyUpdated)
            {
                await Clients.User(receiverId.Value.ToString()).SendAsync("MessagesSeen", new
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
            {
                _userConnectionManager.RemoveConnection(userId, Context.ConnectionId);
                var ActiveConnections = _userConnectionManager.GetConnections(userId);
                if (ActiveConnections == null || ActiveConnections.Count == 0)
                {
                    var user = await userService.GetUserById(userId);
                    if (user != null)
                    {
                        user.SetActive(false);
                        user.UpdateLastSeen(); // Updates LastSeen to DateTime.UtcNow
                        await userService.UpdateUserAsync(user);
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
