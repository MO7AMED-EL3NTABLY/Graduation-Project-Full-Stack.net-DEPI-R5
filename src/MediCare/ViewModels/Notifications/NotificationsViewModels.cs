using MVC.BLL.DTOs.Notifications;
using MVC.DAL.Enums;

namespace MVC.PL.ViewModels.Notifications
{
    public class NotificationsIndexViewModel
    {
        public IReadOnlyList<NotificationDto> Items { get; set; } = Array.Empty<NotificationDto>();
        public int UnreadCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public string CurrentArea { get; set; } = "Patient";
    }

    public class NotificationPreferencesViewModel
    {
        public string CurrentArea { get; set; } = "Patient";
        public List<NotificationPreferenceItemViewModel> Items { get; set; } = new();
    }

    public class NotificationPreferenceItemViewModel
    {
        public NotificationEventType EventType { get; set; }
        public bool EmailEnabled { get; set; } = true;
        public bool InAppEnabled { get; set; } = true;
        public bool SmsEnabled { get; set; } = false;
    }
}