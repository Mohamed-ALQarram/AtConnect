using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.DTOs
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
