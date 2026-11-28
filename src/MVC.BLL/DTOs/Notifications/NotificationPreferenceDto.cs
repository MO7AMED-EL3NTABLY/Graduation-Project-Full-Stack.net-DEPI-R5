using MVC.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.BLL.DTOs.Notifications
{
    public class NotificationPreferenceDto
    {
        public NotificationEventType EventType { get; set; }
        public bool EmailEnabled { get; set; } = true;
        public bool InAppEnabled { get; set; } = true;
        public bool SmsEnabled { get; set; } = false;
    }
}
