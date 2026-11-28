/*——- Omar Hussein ——-*/
using MVC.BLL.Common;
using MVC.DAL.Enums;

namespace MVC.BLL.DTOs.Accounts
{
    // بيانات تسجيل المريض
    public class RegisterPatientDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Password { get; set; } = string.Empty;
        public FileUpload? Photo { get; set; }
    }

    // بيانات تسجيل الدكتور (الخطوات الـ 3 مع بعض)
    public class RegisterDoctorDto
    {
        // 1) Personal
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public FileUpload? Photo { get; set; }

        // 2) Professional
        public Guid SpecialtyId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public int GraduationYear { get; set; }
        public string University { get; set; } = string.Empty;
        public int PracticeStartYear { get; set; }
        public string Bio { get; set; } = string.Empty;

        // 3) Documents
        public FileUpload? License { get; set; }
        public FileUpload? GraduationCertificate { get; set; }
    }

    public class SpecialtyDto
    {
        public Guid SpecialtyId { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
    }

    // حالة الحساب بعد التأكد من الباسورد - بتحدد الـ PL يودّي المستخدم فين
    public enum LoginState
    {
        Inactive,          // الحساب موقوف (IsActive = false)
        NeedsVerification, // لسه مأكدش الإيميل أو الموبايل
        Patient,
        DoctorPending,
        DoctorRejected,
        DoctorApproved,
        Admin
    }

    // حالة تأكيد الإيميل والموبايل لشاشة Verify
    public class VerificationStatusDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool PhoneRequired { get; set; }
        public bool PhoneConfirmed { get; set; }
        public string MaskedPhone { get; set; } = string.Empty;     // الرقم اللي الكود اتبعتله
        public int ResendInSeconds { get; set; }                    // فاضل كام ثانية لحد ما يقدر يطلب كود جديد
        public bool HasActiveCode { get; set; }

        public bool IsComplete => EmailConfirmed && (!PhoneRequired || PhoneConfirmed);
    }

    // حالة الدكتور لصفحات Pending و Rejected
    public class DoctorStatusDto
    {
        public string FullName { get; set; } = string.Empty;
        public ApprovalStatus ApprovalStatus { get; set; }
        public string? RejectionReason { get; set; }
    }

    // الحساب اللي هيعمل Reset للباسورد - بيانات مخفية جزئياً
    public class ResetAccountDto
    {
        public string UserId { get; set; } = string.Empty;
        public string MaskedEmail { get; set; } = string.Empty;
        public string MaskedPhone { get; set; } = string.Empty;
        public bool CanUseSms { get; set; } // SMS للمريض والدكتور بس، وبرقم متأكد
    }

    // بيانات صفحة Settings (المريض والدكتور)
    public class AccountSettingsDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public bool PhoneConfirmed { get; set; }
        public bool HasPhoto { get; set; }
    }
}
/*————*/
