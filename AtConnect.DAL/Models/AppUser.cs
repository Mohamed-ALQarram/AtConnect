namespace AtConnect.DAL.Models
{
    public class AppUser: IdentityUser
    {
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public DateTime LastSeen { get; private set; }
        public bool IsActive { get; private set; }

        public ICollection<Chat> Chats { get; set; }
        public ICollection<DevcieToken> DevcieTokens { get; set; }
        private AppUser() 
        {
            Chats = new List<Chat>();
            DevcieTokens = new List<DevcieToken>();
            LastSeen = DateTime.Now;
        }

        public AppUser(string FirstName, string LastName, string userName, string email, string passwordHash):base(userName, email, passwordHash)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            Chats = new List<Chat>();
            DevcieTokens = new List<DevcieToken>();
            LastSeen = DateTime.Now;
        }

        public void ChangeFirstName(string newName)
        {
            FirstName = newName;
        }

        public void ChangeLastName(string newName)
        {
            LastName = newName;
        }

        public void UpdateLastSeen()
        {
            LastSeen = DateTime.UtcNow;
        }

        public void SetActive(bool active)
        {
            IsActive = active;
        }


    }
}
