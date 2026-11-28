/*——- Omar Hussein ——-*/
using Microsoft.Extensions.Logging;
using MVC.BLL.Interfaces;

namespace MVC.BLL.Services.Infrastructure
{
    // Fake Email sender للتطوير بس: بيكتب الرسالة (واللينك اللي فيها) في الـ Log بدل ما يبعتها
    // ممنوع يتستخدم في الـ Production لأنه بيكتب لينكات وأكواد في الـ Log
    public class LoggingEmailSender : IEmailSender
    {
        private readonly ILogger<LoggingEmailSender> _logger;

        public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string toEmail, string subject, string body)
        {
            _logger.LogWarning("[FAKE EMAIL] To: {To} | Subject: {Subject}\n{Body}", toEmail, subject, body);
            return Task.CompletedTask;
        }
    }

    // Fake SMS sender للتطوير بس: بيكتب الرسالة (والكود اللي فيها) في الـ Log
    public class LoggingSmsSender : ISmsSender
    {
        private readonly ILogger<LoggingSmsSender> _logger;

        public LoggingSmsSender(ILogger<LoggingSmsSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string toPhoneNumber, string message)
        {
            _logger.LogWarning("[FAKE SMS] To: {To} | {Message}", toPhoneNumber, message);
            return Task.CompletedTask;
        }
    }
}
/*————*/
