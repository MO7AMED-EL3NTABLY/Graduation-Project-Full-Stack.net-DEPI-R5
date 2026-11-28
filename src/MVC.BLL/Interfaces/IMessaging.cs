namespace MVC.BLL.Interfaces
{
    // Sending email - SMTP when configured, or the Fake that writes to the log (Development only)
    public interface IEmailSender
    {
        // true = the mail server accepted the email; false = it was not sent
        Task<bool> SendAsync(string toEmail, string subject, string body);
    }

    // Sending SMS - currently a Fake that writes to the log
    public interface ISmsSender
    {
        Task SendAsync(string toPhoneNumber, string message);
    }
}
