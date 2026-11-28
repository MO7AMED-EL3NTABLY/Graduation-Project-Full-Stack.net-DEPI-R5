using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.BLL.DTOs.Notifications;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.PL.ViewModels.Notifications;

namespace MVC.PL.Controllers
{
    // يخدم المريض (/Patient/Notifications) والدكتور (/Doctor/Notifications) - الأدمن مبيستلمش إشعارات
    [Authorize(Roles = AppRoles.Patient + "," + AppRoles.Doctor)]
    [Route("{area:regex(^(Patient|Doctor)$)}/Notifications")]
    [Route("Notifications")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var items = await _notificationService.GetUserInboxAsync(userId, page, 20, cancellationToken);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId, cancellationToken);

            var vm = new NotificationsIndexViewModel
            {
                Items = items,
                UnreadCount = unreadCount,
                CurrentPage = Math.Max(1, page),
                CurrentArea = GetCurrentArea()
            };

            return View(vm);
        }

        [HttpPost("MarkAsRead/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken = default)
        {
            await _notificationService.MarkAsReadAsync(id, GetUserId(), cancellationToken);
            return Redirect($"/{GetCurrentArea()}/Notifications");
        }

        [HttpPost("MarkAllAsRead")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
        {
            await _notificationService.MarkAllAsReadAsync(GetUserId(), cancellationToken);
            return Redirect($"/{GetCurrentArea()}/Notifications");
        }

        [HttpGet("Preferences")]
        public async Task<IActionResult> Preferences(CancellationToken cancellationToken = default)
        {
            var prefs = await _notificationService.GetUserPreferencesAsync(
                GetUserId(),
                User.IsInRole(AppRoles.Doctor),
                cancellationToken);

            var vm = new NotificationPreferencesViewModel
            {
                CurrentArea = GetCurrentArea(),
                Items = prefs.Select(p => new NotificationPreferenceItemViewModel
                {
                    EventType = p.EventType,
                    EmailEnabled = p.EmailEnabled,
                    InAppEnabled = p.InAppEnabled,
                    SmsEnabled = p.SmsEnabled
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost("Preferences")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Preferences(
            NotificationPreferencesViewModel model,
            CancellationToken cancellationToken = default)
        {
            var dtos = model.Items.Select(i => new NotificationPreferenceDto
            {
                EventType = i.EventType,
                EmailEnabled = i.EmailEnabled,
                InAppEnabled = i.InAppEnabled,
                SmsEnabled = i.SmsEnabled
            }).ToList();

            await _notificationService.UpdateUserPreferencesAsync(
                GetUserId(),
                User.IsInRole(AppRoles.Doctor),
                dtos,
                cancellationToken);

            TempData["SuccessMessage"] = "Notification preferences saved.";
            return Redirect($"/{GetCurrentArea()}/Notifications/Preferences");
        }

        private string GetUserId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new UnauthorizedAccessException();

        private string GetCurrentArea()
            => User.IsInRole(AppRoles.Doctor) ? AppRoles.Doctor : AppRoles.Patient;
    }
}