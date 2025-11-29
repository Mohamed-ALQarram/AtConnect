using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpOptions _emailSettings;
        private readonly IConfiguration _configuration;

        public EmailService(IOptions<SmtpOptions> emailSettings, IConfiguration configuration)
        {
            _emailSettings = emailSettings.Value;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(SendEmailDTO sendEmailDTO)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.Email));
            email.To.Add(MailboxAddress.Parse(sendEmailDTO.Email));
            email.Subject = sendEmailDTO.subject;

            // Create a simple text body
            var builder = new BodyBuilder();
            builder.HtmlBody = sendEmailDTO.HtmlBody;
            email.Body = builder.ToMessageBody();

            // Connect to the SMTP server and send
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
