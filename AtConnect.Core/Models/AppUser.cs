namespace AtConnect.Core.Models
{
    public class AppUser: IdentityUser
    {
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string? ImageURL { get; private set; } 
        public DateTime LastSeen { get; private set; }
        public bool IsActive { get; private set; }
        public string? AboutUser { get; private set; }
        public ICollection<Chat> Chats { get; set; }
        public ICollection<ChatRequest>? ChatRequests { get; set; }
        public ICollection<DeviceToken> DeviceTokens { get; set; }
        private AppUser() 
        {
            Chats = new List<Chat>();
            DeviceTokens = new List<DeviceToken>();
            //LastSeen = DateTime.Now;
            LastSeen = DateTime.UtcNow;
        }

        public AppUser(string FirstName, string LastName, string userName, string email, string passwordHash) :base(userName, email, passwordHash)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.ImageURL = "";
            Chats = new List<Chat>();
            DeviceTokens = new List<DeviceToken>();
            LastSeen = DateTime.UtcNow;
        }

        public void ChangeFirstName(string newName)
        {
            FirstName = newName;
        }
        public void ChangeImage(string newUrl)
        {
            ImageURL = newUrl;
        }
        public void ChangeAboutUser(string newAboutContent)
        {
            AboutUser = newAboutContent;
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
