using AtConnect.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.Core.SharedDTOs
{
    public class NotificationDTO
    {
        public int UserId { get; set; }
        public int? ChatId { get; set; }
        public int? RequestId { get; set; }
        public string UserFullName { get; set; }=string.Empty;
        public string? AvatarUrl { get; set; } 
      //  public string Title { get; set; } = string.Empty;
        public string Content { get; set; }= string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public NotificationType notificationType { get; set; }


    }
}
