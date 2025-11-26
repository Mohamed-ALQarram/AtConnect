namespace AtConnect.DAL.Models
{
    public class Chat
    {
        private Chat() { } // EF Core

        public Chat(int user1Id, int user2Id)
        {
            User1Id = user1Id;
            User2Id = user2Id;
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; private set; }
        public int User1Id { get; private set; }
        public int User2Id { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public AppUser User1 { get; private set; } = null!;
        public AppUser User2 { get; private set; } = null!;
        public ICollection<Message> Messages { get; private set; } = new List<Message>();
    }

}