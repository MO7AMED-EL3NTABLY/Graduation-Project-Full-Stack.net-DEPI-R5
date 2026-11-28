using MVC.DAL.Entities.Accounts;
using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Chat;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Notifications
{
    // Notification - Notifications table
    // One row is the event, the in-app inbox entry and the email/SMS delivery tracking
    // CHECK: at most one of AppointmentId, ChatThreadId, ScanOrderId and PrescriptionId has a value
    // Users can't delete notifications
    public class Notification
    {
        public Guid NotificationId { get; set; }

        public string UserId { get; set; } = null!; // FK to AspNetUsers - the recipient (Patient or Doctor)

        public Guid? AppointmentId { get; set; } // For appointment notifications

        public Guid? ChatThreadId { get; set; } // For ChatMessageReceived

        public Guid? ScanOrderId { get; set; } // For ScanOrderCreated

        public Guid? PrescriptionId { get; set; } // For PrescriptionExpiring and MedicationReminder

        public NotificationEventType EventType { get; set; } // Event type

        public string Title { get; set; } = null!; // Notification title

        public string Body { get; set; } = null!; // Notification body

        public bool IsRead { get; set; } // Read or not

        public bool InAppEnabled { get; set; } // Shown in the inbox or not (based on the settings at creation time)

        public DateTimeOffset CreatedAtUtc { get; set; } // Creation time

        public DeliveryStatus EmailStatus { get; set; } // Email delivery status

        public int EmailRetryCount { get; set; } // Email attempts (max 3)

        public string? EmailLastError { get; set; } // Last email error - cleared on success

        public DeliveryStatus SmsStatus { get; set; } // SMS delivery status

        public int SmsRetryCount { get; set; } // SMS attempts (max 3)

        public string? SmsLastError { get; set; } // Last SMS error - cleared on success

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Appointment? Appointment { get; set; }
        public ChatThread? ChatThread { get; set; }
        public ScanOrder? ScanOrder { get; set; }
        public Prescription? Prescription { get; set; }
    }
}
