using MVC.DAL.Enums;

namespace MVC.BLL.DTOs.Notifications
{
    // شكل الإشعار اللي بيرجع للواجهة عشان يتعرض في صندوق الإشعارات (InApp Inbox)
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public NotificationEventType EventType { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }

        // عشان لما المستخدم يضغط على الإشعار يوديه لصفحة المصدر
        public Guid? AppointmentId { get; set; }
        public Guid? ChatThreadId { get; set; }
        public Guid? ScanOrderId { get; set; }
        public Guid? PrescriptionId { get; set; }
    }
}