using AtConnect.DAL.Enum;

namespace AtConnect.DAL.Models
{
    public class ChatRequest
    {
        private ChatRequest() { }

        public ChatRequest(int senderId, int receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            CreatedAt = DateTime.UtcNow;
            Status = RequestStatus.Pending;
        }

        public int Id { get; private set; }
        public int SenderId { get; private set; }
        public int ReceiverId { get; private set; }
        public RequestStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public AppUser Sender { get; private set; } = null!;
        public AppUser Receiver { get; private set; } = null!;

        public void Accept() => Status = RequestStatus.Accepted;
        public void Reject() => Status = RequestStatus.Rejected;
    }

}