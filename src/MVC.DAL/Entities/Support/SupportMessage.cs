using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Support
{
    // A message in the support conversation - SupportMessages table
    // The sender must be the conversation owner or an admin (checked in the BLL), and there is no read tracking
    public class SupportMessage
    {
        public Guid SupportMessageId { get; set; }

        public Guid SupportConversationId { get; set; } // FK to SupportConversations

        public string SenderUserId { get; set; } = null!; // FK to AspNetUsers - Patient, Doctor or Admin

        public string Text { get; set; } = null!; // Message text

        public DateTimeOffset SentAtUtc { get; set; } // Sent time

        // Navigation properties
        public SupportConversation SupportConversation { get; set; } = null!;
        public ApplicationUser SenderUser { get; set; } = null!;
    }
}
