using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.DTOs
{
    public class SendEmailDTO
    {
        public string Email { get; set; }=string.Empty;
        public string subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;

        public SendEmailDTO(string email, string subject, string htmlBody)
        {
            Email = email;
            this.subject = subject;
            HtmlBody = htmlBody;
        }
    }
}
