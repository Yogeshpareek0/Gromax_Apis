using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace GromaxMobileApis.Utilities
{
    public class EmailService
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class EmailRequest
        {
            public List<string> To { get; set; } = new() { "ykp900155@gmail.com" };
            public List<string> Cc { get; set; } = new();
            public List<string> Bcc { get; set; } = new();

            public string Subject { get; set; }
            public string Body { get; set; }
            public bool IsHtml { get; set; } = true;
        }

        public static async Task<bool> SendEmailAsync(EmailRequest request)
        {
            try
            {
                if (request == null || request.To == null || !request.To.Any())
                    throw new ArgumentException("Recipient email is required.");

                var settings = _configuration.GetSection("EmailSettings");

                string host = settings["SmtpHost"];
                int port = Convert.ToInt32(settings["SmtpPort"]);
                string fromEmail = settings["FromEmail"];
                string fromName = settings["FromName"];
                string password = settings["Password"];

                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(fromName, fromEmail));

                foreach (var email in request.To)
                    message.To.Add(MailboxAddress.Parse(email));

                foreach (var email in request.Cc)
                    message.Cc.Add(MailboxAddress.Parse(email));

                foreach (var email in request.Bcc)
                    message.Bcc.Add(MailboxAddress.Parse(email));

                message.Subject = request.Subject;

                var bodyBuilder = new BodyBuilder();

                if (request.IsHtml)
                    bodyBuilder.HtmlBody = request.Body;
                else
                    bodyBuilder.TextBody = request.Body;

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                client.CheckCertificateRevocation = false;

                await client.ConnectAsync(
                    host,
                    port,
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(fromEmail, password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}