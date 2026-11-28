using Microsoft.Extensions.Logging;
using MVC.BLL.Interfaces;

namespace MVC.BLL.Services.Infrastructure
{
    // Fake email sender for development only: writes the message (and its link) to the log instead of sending it
    // Must not be used in production because it writes links and codes to the log
    public class LoggingEmailSender : IEmailSender
    {
        private readonly ILogger<LoggingEmailSender> _logger;

        public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendAsync(string toEmail, string subject, string body)
        {
            _logger.LogWarning("[FAKE EMAIL] To: {To} | Subject: {Subject}\n{Body}", toEmail, subject, body);
            return Task.FromResult(true);
        }
    }

    // Fake SMS sender for development only: writes the message (and its code) to the log
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
