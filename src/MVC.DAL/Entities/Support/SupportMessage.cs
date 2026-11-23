using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Support
{
    // رسالة في محادثة الدعم - جدول SupportMessages
    // الراسل لازم يكون صاحب المحادثة أو أدمن (بيتأكد في الـ BLL)، ومفيش تتبع قراية
    public class SupportMessage
    {
        public Guid SupportMessageId { get; set; }

        public Guid SupportConversationId { get; set; } // FK لـ SupportConversations

        public string SenderUserId { get; set; } = null!; // FK لـ AspNetUsers - Patient أو Doctor أو Admin

        public string Text { get; set; } = null!; // نص الرسالة

        public DateTimeOffset SentAtUtc { get; set; } // وقت الإرسال

        // Navigation properties
        public SupportConversation SupportConversation { get; set; } = null!;
        public ApplicationUser SenderUser { get; set; } = null!;
    }
}
