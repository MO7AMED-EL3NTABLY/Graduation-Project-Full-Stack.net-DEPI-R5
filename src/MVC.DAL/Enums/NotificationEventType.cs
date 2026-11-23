namespace MVC.DAL.Enums
{
    // نوع الحدث اللي بيعمل إشعار - Notifications.EventType و NotificationPreferences.EventType
    public enum NotificationEventType
    {
        AppointmentConfirmed, // تأكيد حجز
        AppointmentCancelled, // إلغاء حجز
        AppointmentReminder, // تذكير بميعاد الحجز
        MedicationReminder, // تذكير بميعاد الدوا
        PrescriptionExpiring, // الروشتة قربت تختفي
        ScanOrderCreated, // الدكتور طلب تحليل أو أشعة
        ChatMessageReceived, // رسالة شات جديدة
        SupportMessageReceived // رد جديد من الدعم
    }
}
