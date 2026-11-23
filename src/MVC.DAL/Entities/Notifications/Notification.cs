using MVC.DAL.Entities.Accounts;
using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Chat;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Notifications
{
    // إشعار - جدول Notifications
    // صف واحد بيمثل الحدث وصندوق الإشعارات جوه التطبيق وتتبع إرسال الإيميل والـ SMS
    // CHECK: واحد بالكتير من AppointmentId و ChatThreadId و ScanOrderId و PrescriptionId يكون له قيمة
    // المستخدمين مش بيقدروا يمسحوا الإشعارات
    public class Notification
    {
        public Guid NotificationId { get; set; }

        public string UserId { get; set; } = null!; // FK لـ AspNetUsers - المستلم (Patient أو Doctor)

        public Guid? AppointmentId { get; set; } // للإشعارات الخاصة بالحجز

        public Guid? ChatThreadId { get; set; } // لـ ChatMessageReceived

        public Guid? ScanOrderId { get; set; } // لـ ScanOrderCreated

        public Guid? PrescriptionId { get; set; } // لـ PrescriptionExpiring و MedicationReminder

        public NotificationEventType EventType { get; set; } // نوع الحدث

        public string Title { get; set; } = null!; // عنوان الإشعار

        public string Body { get; set; } = null!; // نص الإشعار

        public bool IsRead { get; set; } // اتقرا ولا لأ

        public bool InAppEnabled { get; set; } // يظهر في صندوق الإشعارات ولا لأ (حسب الإعدادات وقت الإنشاء)

        public DateTimeOffset CreatedAtUtc { get; set; } // وقت الإنشاء

        public DeliveryStatus EmailStatus { get; set; } // حالة إرسال الإيميل

        public int EmailRetryCount { get; set; } // عدد محاولات الإيميل (أقصى 3)

        public string? EmailLastError { get; set; } // آخر خطأ في الإيميل - بيتمسح لما ينجح

        public DeliveryStatus SmsStatus { get; set; } // حالة إرسال الـ SMS

        public int SmsRetryCount { get; set; } // عدد محاولات الـ SMS (أقصى 3)

        public string? SmsLastError { get; set; } // آخر خطأ في الـ SMS - بيتمسح لما ينجح

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Appointment? Appointment { get; set; }
        public ChatThread? ChatThread { get; set; }
        public ScanOrder? ScanOrder { get; set; }
        public Prescription? Prescription { get; set; }
    }
}
