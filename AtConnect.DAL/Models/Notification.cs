using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.DAL.Models
{
    public class Notification
    {
        private Notification() { } // EF Core

        public Notification(int userId, string message)
        {
            UserId = userId;
            Message = message;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }

        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Message { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public bool IsRead { get; private set; }

        public AppUser User { get; private set; } = null!;

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }

}
