using AtConnect.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.Core.Models
{
    public class Notification
    {
        private Notification() { } 

        public Notification(int receiverId, int senderId, int? chatId, int? chatRequestId, string message, NotificationType type)
        {
            ReceiverId = receiverId;
            SenderId = senderId;
            ChatId = chatId;
            ChatRequestId = chatRequestId;
            Message = message;
            Type = type;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }

        public int Id { get; private set; }
        public int ReceiverId { get; private set; }
        public int SenderId { get; private set; }
        public int? ChatId { get; private set; }
        public int? ChatRequestId { get; private set; }
        public string Message { get; private set; } = null!;
        public NotificationType Type { get; private set; } 
        public DateTime CreatedAt { get; private set; }
        public bool IsRead { get; private set; }
        public AppUser Receiver { get; private set; } = null!;
        public AppUser Sender { get; private set; } = null!;
        public Chat? Chat { get; private set; }
        public ChatRequest? ChatRequest { get; private set; }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
    
}
