using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Chat
{
    // رسالة في محادثة مريض ودكتور - جدول ChatMessages
    // CHECK: لازم يكون فيها نص أو ملف أو الاتنين
    // الرسايل مش بتتمسح، والـ BLL بيتأكد إن الراسل هو مريض أو دكتور المحادثة
    public class ChatMessage
    {
        public Guid ChatMessageId { get; set; }

        public Guid ChatThreadId { get; set; } // FK لـ ChatThreads

        public string SenderUserId { get; set; } = null!; // FK لـ AspNetUsers - الراسل

        public string? Text { get; set; } // نص الرسالة

        public string? AttachmentStorageKey { get; set; } // مفتاح الملف المرفق

        public string? OriginalFileName { get; set; } // اسم الملف الأصلي

        public string? ContentType { get; set; } // نوع الملف

        public long? FileSizeBytes { get; set; } // حجم الملف بالبايت

        public bool IsRead { get; set; } // اتقرت ولا لأ (من غير وقت القراية)

        public DateTimeOffset SentAtUtc { get; set; } // وقت الإرسال

        // Navigation properties
        public ChatThread ChatThread { get; set; } = null!;
        public ApplicationUser SenderUser { get; set; } = null!;
    }
}
