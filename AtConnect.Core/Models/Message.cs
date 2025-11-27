using AtConnect.Core.Enum;

namespace AtConnect.Core.Models
{
    public class Message
    {
        private Message() { }

        public Message(int senderId, int chatId, string content)
        {
            SenderId = senderId;
            ChatId = chatId;
            Content = content;
            SentAt = DateTime.UtcNow;
            Status = MessageStatus.Sent;
        }

        public int Id { get; private set; }
        public int SenderId { get; private set; }
        public int ChatId { get; private set; }
        public string Content { get; private set; } = null!;
        public DateTime SentAt { get; private set; }
        public MessageStatus Status { get; private set; }

        public AppUser Sender { get; private set; } = null!;
        public Chat Chat { get; private set; } = null!;

        public void MarkAsDelivered() => Status = MessageStatus.Delivered;
        public void MarkAsRead() => Status = MessageStatus.Seen;
    }

}