using AtConnect.Core.Enum;
using AtConnect.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.DTOs
{
    public class UserListItemDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ProfilePhotoUrl { get; set; } = string.Empty;
        public bool isActive { get; set; }
        public ChatRequest? ChatRequest{ get; set; }
    }
}
