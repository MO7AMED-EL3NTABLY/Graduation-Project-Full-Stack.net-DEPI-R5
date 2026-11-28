using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MVC.BLL.Interfaces;

namespace MVC.BLL.Services.Infrastructure
{
    // إعدادات الـ SMTP - بتتقرا من User Secrets (قسم Smtp)، مفيش باسورد في الكود
    public class SmtpOptions
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromName { get; set; } = "MediCare";
    }

    // بيبعت إيميل حقيقي عن طريق Gmail SMTP (App Password)
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_options.Username, _options.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true // الـ body جاي من EmailTemplates
            };
            message.To.Add(toEmail);

            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };

            try
            {
                await client.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                // الحساب بيكون اتحفظ خلاص، فمش عايزين الصفحة تقع - المستخدم يقدر يدوس Resend
                // بنسجل الإيميل والخطأ بس، مش محتوى الرسالة (فيها لينكات) - Spec §12
                _logger.LogError(ex, "Sending email to {To} failed.", toEmail);
            }
        }
    }
}
