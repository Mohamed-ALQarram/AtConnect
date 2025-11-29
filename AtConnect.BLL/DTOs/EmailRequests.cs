using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.DTOs
{
    public record EmailVerificationRequest(string Email);
    public record ConfirmEmailVerificationRequest(string Email, string Token);
    public record SendEmailRequest(string Email, string Subject, string HtmlBody);
}
