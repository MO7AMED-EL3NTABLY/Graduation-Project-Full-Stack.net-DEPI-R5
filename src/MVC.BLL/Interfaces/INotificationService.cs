using MVC.BLL.DTOs.Notifications;

namespace MVC.BLL.Interfaces
{
    // العقد الرئيسي للإشعارات - عضو 3 وعضو 4 هيحقنوا الواجهة دي في الـ Services بتاعتهم
    public interface INotificationService
    {
        // إنشاء إشعار جديد حسب إعدادات المستخدم (saveChanges = false لو جوه Transaction خارجية)
        Task<Guid?> CreateAsync(
            CreateNotificationDto dto,
            bool saveChanges = true,
            CancellationToken cancellationToken = default);

        // مسح إشعارات ميعاد قديم عند نجاح تغيير الميعاد (Reschedule) - هيحتاجها عضو 3
        Task DeleteAppointmentNotificationsAsync(
            Guid appointmentId,
            bool saveChanges = true,
            CancellationToken cancellationToken = default);

        // جلب إشعارات صندوق التطبيق (InApp) لمستخدم معين
        Task<IReadOnlyList<NotificationDto>> GetUserInboxAsync(
            string userId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        // عدد الإشعارات غير المقروءة (للجرس اللي في الـ Topbar)
        Task<int> GetUnreadCountAsync(
            string userId,
            CancellationToken cancellationToken = default);

        // تعليم إشعار إنه اتقرا
        Task<bool> MarkAsReadAsync(
            Guid notificationId,
            string userId,
            CancellationToken cancellationToken = default);

        // تعليم كل الإشعارات غير المقروءة كمقروءة
        Task MarkAllAsReadAsync(
            string userId,
            CancellationToken cancellationToken = default);

        // قراءة إعدادات الإشعارات (بالقيم الافتراضية لو المستخدم لسه مغيرهاش)
        Task<IReadOnlyList<NotificationPreferenceDto>> GetUserPreferencesAsync(
            string userId,
            bool isDoctor,
            CancellationToken cancellationToken = default);

        // حفظ تعديلات المستخدم لإعدادات الإشعارات
        Task UpdateUserPreferencesAsync(
            string userId,
            bool isDoctor,
            IReadOnlyList<NotificationPreferenceDto> preferences,
            CancellationToken cancellationToken = default);
    }
}