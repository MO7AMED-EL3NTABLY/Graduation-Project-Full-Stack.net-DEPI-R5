using MVC.DAL.Entities.Accounts;
using MVC.DAL.Entities.Notifications;

namespace MVC.DAL.Entities.Chat
{
    // A permanent conversation between a patient and a doctor - ChatThreads table
    // Unique on (PatientId, DoctorId) - created when their first appointment becomes Completed
    public class ChatThread
    {
        public Guid ChatThreadId { get; set; }

        public string PatientId { get; set; } = null!; // FK to PatientProfiles

        public string DoctorId { get; set; } = null!; // FK to DoctorProfiles

        public DateTimeOffset CreatedAtUtc { get; set; } // Creation time

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile Doctor { get; set; } = null!;
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
