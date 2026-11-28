using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MVC.BLL.Interfaces;

namespace MVC.BLL.Services.Infrastructure
{
    // SMTP settings - read from User Secrets (Smtp section), no password in the code
    public class SmtpOptions
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromName { get; set; } = "MediCare";
    }

    // Sends a real email through Gmail SMTP (App Password)
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        // Returns false when the email was not sent, so the calling service can report the failure
        public async Task<bool> SendAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Built inside the try: an invalid recipient address throws FormatException here
                using var message = new MailMessage
                {
                    From = new MailAddress(_options.Username, _options.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // The body comes from EmailTemplates
                };
                message.To.Add(toEmail);

                using var client = new SmtpClient(_options.Host, _options.Port)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_options.Username, _options.Password)
                };

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex) when (ex is SmtpException or InvalidOperationException or FormatException)
            {
                // Log only the email and the error, not the message content (it has links) - Spec §12
                _logger.LogError(ex, "Sending email to {To} failed.", toEmail);
                return false;
            }
        }
    }
}
