using MVC.DAL.Entities.Accounts;
using MVC.DAL.Entities.Notifications;

namespace MVC.DAL.Entities.Chat
{
    // محادثة دايمة بين مريض ودكتور - جدول ChatThreads
    // Unique على (PatientId, DoctorId) - بتتعمل لما أول حجز بينهم يبقى Completed
    public class ChatThread
    {
        public Guid ChatThreadId { get; set; }

        public string PatientId { get; set; } = null!; // FK لـ PatientProfiles

        public string DoctorId { get; set; } = null!; // FK لـ DoctorProfiles

        public DateTimeOffset CreatedAtUtc { get; set; } // وقت الإنشاء

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile Doctor { get; set; } = null!;
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
