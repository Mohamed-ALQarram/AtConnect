using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.DTOs
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }=string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; } 
        public int RefreshTokenExpiresIn { get; set; }
    }
}

