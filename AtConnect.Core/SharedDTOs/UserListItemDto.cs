using AtConnect.Core.Models;

namespace AtConnect.Core.SharedDTOs
{
    public class UserListItemDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ProfilePhotoUrl { get; set; } = string.Empty;
        public bool isActive { get; set; }
        public string AboutUser { get; set; } = string.Empty;
        public ChatRequest? ChatRequest{ get; set; }
    }
}
