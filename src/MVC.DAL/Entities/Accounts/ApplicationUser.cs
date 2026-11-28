using Microsoft.AspNetCore.Identity;
using MVC.DAL.Entities.Notifications;
using MVC.DAL.Entities.Support;

namespace MVC.DAL.Entities.Accounts
{
    // A user on the platform (Patient, Doctor or Admin) - AspNetUsers table
    // Inherits from IdentityUser, which has these basic columns:
    // Id, UserName, Email, PhoneNumber, PasswordHash, Lockout

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!; // Full name

        public string? ProfilePhotoStorageKey { get; set; } // Profile photo key (null for the admin only)

        public bool IsActive { get; set; } // Account activation - the only activation/suspension flag

        // Navigation properties
        public PatientProfile? PatientProfile { get; set; } // Exists only if the account is a Patient
        public DoctorProfile? DoctorProfile { get; set; } // Exists only if the account is a Doctor
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<PhoneVerificationCode> PhoneVerificationCodes { get; set; } = new List<PhoneVerificationCode>();
        public SupportConversation? SupportConversation { get; set; } // Support conversation (Patient and Doctor only)
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<NotificationPreference> NotificationPreferences { get; set; } = new List<NotificationPreference>();
    }
}
