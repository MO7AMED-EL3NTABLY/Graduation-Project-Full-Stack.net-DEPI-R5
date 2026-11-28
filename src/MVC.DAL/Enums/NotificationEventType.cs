namespace MVC.DAL.Enums
{
    // Event type that creates a notification - Notifications.EventType and NotificationPreferences.EventType
    public enum NotificationEventType
    {
        AppointmentConfirmed, // Booking confirmed
        AppointmentCancelled, // Booking cancelled
        AppointmentReminder, // Appointment reminder
        MedicationReminder, // Medicine time reminder
        PrescriptionExpiring, // The prescription is about to be hidden
        ScanOrderCreated, // The doctor ordered a lab test or scan
        ChatMessageReceived, // New chat message
        SupportMessageReceived // New reply from support
    }
}
