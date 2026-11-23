using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Support
{
    // محادثة الدعم الدايمة لكل حساب Patient أو Doctor - جدول SupportConversations
    // Unique على UserId - مفيش tickets ولا حالة ولا قفل
    public class SupportConversation
    {
        public Guid SupportConversationId { get; set; }

        public string UserId { get; set; } = null!; // FK لـ AspNetUsers - صاحب المحادثة (unique)

        public DateTimeOffset CreatedAtUtc { get; set; } // وقت الإنشاء

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public ICollection<SupportMessage> Messages { get; set; } = new List<SupportMessage>();
    }
}
