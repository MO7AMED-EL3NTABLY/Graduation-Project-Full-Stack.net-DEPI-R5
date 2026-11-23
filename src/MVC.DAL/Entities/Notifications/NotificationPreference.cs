using MVC.DAL.Entities.Accounts;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Notifications
{
    // إعدادات الإشعارات لكل مستخدم ونوع حدث وقناة - جدول NotificationPreferences
    // Composite PK (UserId, EventType, Channel) ومفيش Id منفصل
    // الافتراضي: Email و InApp شغالين - SMS مقفول
    public class NotificationPreference
    {
        public string UserId { get; set; } = null!; // FK لـ AspNetUsers

        public NotificationEventType EventType { get; set; } // نوع الحدث

        public NotificationChannel Channel { get; set; } // Email / SMS / InApp

        public bool IsEnabled { get; set; } // القناة دي شغالة للحدث ده ولا لأ

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
    }
}
