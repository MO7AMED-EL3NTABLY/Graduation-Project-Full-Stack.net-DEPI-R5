using MVC.DAL.Enums;

namespace MVC.BLL.DTOs.Notifications
{
    // البيانات المطلوبة لإنشاء إشعار جديد (بيناديها عضو 3 وعضو 4 والشات والدعم)
    public class CreateNotificationDto
    {
        // Id المستخدم المستلم (مريض أو دكتور فقط - الأدمن مبياخدش إشعارات)
        public string UserId { get; set; } = null!;

        // نوع الحدث (من الـ 8 أنواع الموجودة في MVC.DAL.Enums.NotificationEventType)
        public NotificationEventType EventType { get; set; }

        // عنوان الإشعار ونصه
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;

        // مصدر الإشعار: حسب قاعدة الداتابيز لازم واحد بس بالكتير يكون له قيمة (والباقي null)
        public Guid? AppointmentId { get; set; }
        public Guid? ChatThreadId { get; set; }
        public Guid? ScanOrderId { get; set; }
        public Guid? PrescriptionId { get; set; }
    }
}