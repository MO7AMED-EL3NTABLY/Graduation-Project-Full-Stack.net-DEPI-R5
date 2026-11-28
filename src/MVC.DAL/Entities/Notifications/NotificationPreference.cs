using MVC.DAL.Entities.Accounts;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Notifications
{
    // Notification settings per user, event type and channel - NotificationPreferences table
    // Composite PK (UserId, EventType, Channel) with no separate Id
    // Default: Email and InApp on - SMS off
    public class NotificationPreference
    {
        public string UserId { get; set; } = null!; // FK to AspNetUsers

        public NotificationEventType EventType { get; set; } // Event type

        public NotificationChannel Channel { get; set; } // Email / SMS / InApp

        public bool IsEnabled { get; set; } // Whether this channel is on for this event

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
    }
}
