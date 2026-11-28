using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Support
{
    // The permanent support conversation of each Patient or Doctor account - SupportConversations table
    // Unique on UserId - no tickets, statuses or closing
    public class SupportConversation
    {
        public Guid SupportConversationId { get; set; }

        public string UserId { get; set; } = null!; // FK to AspNetUsers - the conversation owner (unique)

        public DateTimeOffset CreatedAtUtc { get; set; } // Creation time

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public ICollection<SupportMessage> Messages { get; set; } = new List<SupportMessage>();
    }
}
