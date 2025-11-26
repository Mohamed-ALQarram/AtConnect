using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.DAL.Models
{
    public class DevcieToken
    {
        private DevcieToken() { } 

        public DevcieToken(int userId, string fcmToken)
        {
            UserId = userId;
            Token = fcmToken;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Token { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        public AppUser User { get; private set; } = null!;

        public void SetActive(bool active)
        {
            IsActive = active;
        }

        public void UpdateToken(string newToken)
        {
            Token = newToken;
        }

    }
}
