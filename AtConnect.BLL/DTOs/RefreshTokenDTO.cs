using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.DTOs
{
    public class RefreshTokenDTO
    {
        public string OldToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
