using Microsoft.EntityFrameworkCore;
using MVC.BLL.DTOs.Notifications;
using MVC.BLL.Interfaces;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Entities.Notifications;
using MVC.DAL.Enums;

namespace MVC.BLL.Services
{
    // التنفيذ المبدئي الشغال لخدمة الإشعارات (بيستخدم MediCareDbContext مباشرة بدون Repository)
    public class NotificationService : INotificationService
    {
        private readonly MediCareDbContext _dbContext;

        public NotificationService(MediCareDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid?> CreateAsync(
            CreateNotificationDto dto,
            bool saveChanges = true,
            CancellationToken cancellationToken = default)
        {
            // 1) التأكد من قاعدة الداتابيز (CK_Notifications_SingleSource): مصدر واحد بس بالكتير
            int sourcesCount =
                (dto.AppointmentId.HasValue ? 1 : 0) +
                (dto.ChatThreadId.HasValue ? 1 : 0) +
                (dto.ScanOrderId.HasValue ? 1 : 0) +
                (dto.PrescriptionId.HasValue ? 1 : 0);

            if (sourcesCount > 1)
                throw new InvalidOperationException("الإشعار مينفعش يرتبط بأكتر من مصدر واحد في نفس الوقت.");

            // 2) قراءة إعدادات المستخدم للحدث ده (الافتراضي: Email و InApp شغالين، SMS مقفول)
            var preferences = await _dbContext.NotificationPreferences
                .AsNoTracking()
                .Where(p => p.UserId == dto.UserId && p.EventType == dto.EventType)
                .ToListAsync(cancellationToken);

            bool inAppEnabled = preferences
                .FirstOrDefault(p => p.Channel == NotificationChannel.InApp)?.IsEnabled ?? true;

            bool emailEnabled = preferences
                .FirstOrDefault(p => p.Channel == NotificationChannel.Email)?.IsEnabled ?? true;

            bool smsEnabled = preferences
                .FirstOrDefault(p => p.Channel == NotificationChannel.SMS)?.IsEnabled ?? false;

            // 3) حسب القاعدة: لو التلات قنوات مقفولين منعملش صف في الجدول أصلاً
            if (!inAppEnabled && !emailEnabled && !smsEnabled)
                return null;

            // 4) إنشاء صف الإشعار (إرسال Email و SMS الفعلي هيتعمل بالـ Worker في PR تاني بعد الـ Commit)
            var notification = new Notification
            {
                UserId = dto.UserId,
                EventType = dto.EventType,
                Title = dto.Title,
                Body = dto.Body,
                AppointmentId = dto.AppointmentId,
                ChatThreadId = dto.ChatThreadId,
                ScanOrderId = dto.ScanOrderId,
                PrescriptionId = dto.PrescriptionId,
                IsRead = false,
                InAppEnabled = inAppEnabled,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                EmailStatus = emailEnabled ? DeliveryStatus.Pending : DeliveryStatus.NotRequired,
                EmailRetryCount = 0,
                SmsStatus = smsEnabled ? DeliveryStatus.Pending : DeliveryStatus.NotRequired,
                SmsRetryCount = 0
            };

            _dbContext.Notifications.Add(notification);

            if (saveChanges)
                await _dbContext.SaveChangesAsync(cancellationToken);

            return notification.NotificationId;
        }

        public async Task DeleteAppointmentNotificationsAsync(
            Guid appointmentId,
            bool saveChanges = true,
            CancellationToken cancellationToken = default)
        {
            var oldNotifications = await _dbContext.Notifications
                .Where(n => n.AppointmentId == appointmentId)
                .ToListAsync(cancellationToken);

            if (oldNotifications.Count == 0)
                return;

            _dbContext.Notifications.RemoveRange(oldNotifications);

            if (saveChanges)
                await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<NotificationDto>> GetUserInboxAsync(
            string userId,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            return await _dbContext.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId && n.InAppEnabled)
                .OrderByDescending(n => n.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    EventType = n.EventType,
                    Title = n.Title,
                    Body = n.Body,
                    IsRead = n.IsRead,
                    CreatedAtUtc = n.CreatedAtUtc,
                    AppointmentId = n.AppointmentId,
                    ChatThreadId = n.ChatThreadId,
                    ScanOrderId = n.ScanOrderId,
                    PrescriptionId = n.PrescriptionId
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUnreadCountAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Notifications
                .AsNoTracking()
                .CountAsync(n => n.UserId == userId && n.InAppEnabled && !n.IsRead, cancellationToken);
        }

        public async Task<bool> MarkAsReadAsync(
            Guid notificationId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(
                    n => n.NotificationId == notificationId && n.UserId == userId && n.InAppEnabled,
                    cancellationToken);

            if (notification is null)
                return false;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }
}