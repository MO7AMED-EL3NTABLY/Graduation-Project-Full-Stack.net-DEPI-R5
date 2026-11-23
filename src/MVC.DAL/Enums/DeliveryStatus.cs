namespace MVC.DAL.Enums
{
    // حالة إرسال الإيميل أو الـ SMS - Notifications.EmailStatus و SmsStatus
    public enum DeliveryStatus
    {
        NotRequired, // القناة دي مقفولة ومش محتاجة إرسال
        Pending, // مستني الإرسال
        Sent, // اتبعت
        Failed // فشل بعد 3 محاولات
    }
}
