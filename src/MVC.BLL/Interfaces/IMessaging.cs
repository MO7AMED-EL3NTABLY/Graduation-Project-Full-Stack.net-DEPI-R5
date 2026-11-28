/*——- Omar Hussein ——-*/
namespace MVC.BLL.Interfaces
{
    // إرسال الإيميل - دلوقتي Fake بيكتب في الـ Log، وبعدين نبدله بـ Provider حقيقي من غير ما نغير حاجة تانية
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string body);
    }

    // إرسال الـ SMS - دلوقتي Fake بيكتب في الـ Log
    public interface ISmsSender
    {
        Task SendAsync(string toPhoneNumber, string message);
    }
}
/*————*/
