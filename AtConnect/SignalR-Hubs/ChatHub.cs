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
        private readonly UserManager<AppUser> _userManager;
        public AtConnectHub(IUserConnectionManager userConnectionManager, IChatService chatService, INotificationService notificationService, UserManager<AppUser> userManager, IUserService userService)
        {
            _userConnectionManager = userConnectionManager;
            this.chatService = chatService;
            this.notificationService = notificationService;
            _userManager = userManager;
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
            var activeConnections = _userConnectionManager.GetConnections(userId);
            if(activeConnections == null || activeConnections.Count == 0)
            {
               var user = await _userManager.FindByIdAsync(userId.ToString());
                if(user != null)
                {
                    user.SetActive( true);
                    await _userManager.UpdateAsync(user);
                } 
            }
            if (userId >0)
                _userConnectionManager.AddConnection(userId, Context.ConnectionId);

            await base.OnConnectedAsync();
            
        }
        public async Task SendTyping(int chatId)
        {
            int userId = getUserId();

            if (userId > 0)
            {
                await Clients.GroupExcept(chatId.ToString(), Context.ConnectionId)
                             .SendAsync("ReceiveTyping", new { userId, chatId });
            }
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
            
            // Parallel fetch: receiver validation + sender info (both independent)
            var receiverTask = chatService.GetOtherParticipantIdAsync(msgRequest.ChatId, userId);
            var userTask = userService.GetUserById(userId);
            
            await Task.WhenAll(receiverTask, userTask);
            
            int? receiverId = await receiverTask;
            var user = await userTask;
            
            if (receiverId == null)
                throw new HubException("Not allowed to send messages in this chat.");
            
            if (user == null)
                throw new HubException("Sender user not found.");

            var message = new Message(userId, msgRequest.ChatId, msgRequest.Content);
            await chatService.SaveChatMessage(message);

            // Parallel: broadcast message + persist notification (independent operations)
            var broadcastTask = Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", 
                new { MessageId = message.Id, message.SenderId, message.ChatId, message.Content, message.SentAt });
            
            var notification = new Notification(receiverId.Value, userId, msgRequest.ChatId, null, message.Content, NotificationType.NewMessage);
            var notifyTask = notificationService.AddNotificationAsync(notification);
            
            await Task.WhenAll(broadcastTask, notifyTask);

            // Send real-time notification to receiver
            await Clients.User(receiverId.Value.ToString()).SendAsync("ReceiveNotification", new NotificationDTO
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
            });
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
            //int.TryParse(Context.UserIdentifier, out int userId);
            int userId = getUserId();
            if (userId > 0)
            {
                _userConnectionManager.RemoveConnection(userId, Context.ConnectionId);
                var ActiveConnections = _userConnectionManager.GetConnections(userId);
                if (ActiveConnections == null || ActiveConnections.Count == 0)
                {
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user != null)
                    {
                        user.SetActive(false);
                        user.UpdateLastSeen(); // Updates LastSeen to DateTime.UtcNow
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
