using AtConnect.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(SendEmailRequest emailDTO);
    }
}
