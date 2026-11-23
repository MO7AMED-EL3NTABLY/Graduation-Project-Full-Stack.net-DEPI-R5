using Microsoft.AspNetCore.Identity;
using MVC.DAL.Entities.Notifications;
using MVC.DAL.Entities.Support;

namespace MVC.DAL.Entities.Accounts
{
    // المستخدم في المنصة (Patient أو Doctor أو Admin) - جدول AspNetUsers
    // بيورث من IdentityUser، واللي فيه الأعمدة الأساسية دي:
    // Id, UserName, Email, PhoneNumber, PasswordHash, Lockout

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!; // الاسم الكامل

        public string? ProfilePhotoStorageKey { get; set; } // مفتاح صورة البروفايل (null للأدمن بس)

        public bool IsActive { get; set; } // تفعيل الحساب - الفلاج الوحيد للتفعيل/الإيقاف

        // Navigation properties
        public PatientProfile? PatientProfile { get; set; } // موجود لو الحساب Patient بس
        public DoctorProfile? DoctorProfile { get; set; } // موجود لو الحساب Doctor بس
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<PhoneVerificationCode> PhoneVerificationCodes { get; set; } = new List<PhoneVerificationCode>();
        public SupportConversation? SupportConversation { get; set; } // محادثة الدعم (Patient و Doctor بس)
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<NotificationPreference> NotificationPreferences { get; set; } = new List<NotificationPreference>();
    }
}
