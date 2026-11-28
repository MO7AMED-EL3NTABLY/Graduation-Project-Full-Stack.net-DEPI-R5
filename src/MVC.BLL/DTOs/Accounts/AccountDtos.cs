using MVC.BLL.Common;
using MVC.DAL.Enums;

namespace MVC.BLL.DTOs.Accounts
{
    // Patient registration data
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

    // Doctor registration data (all 3 steps together)
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

    // Account state after the password check - decides where the PL sends the user
    public enum LoginState
    {
        Inactive,          // Account suspended (IsActive = false)
        NeedsVerification, // Email or phone not verified yet
        Patient,
        DoctorPending,
        DoctorRejected,
        DoctorApproved,
        Admin
    }

    // Email and phone verification state for the Verify screen
    public class VerificationStatusDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool PhoneRequired { get; set; }
        public bool PhoneConfirmed { get; set; }
        public string MaskedPhone { get; set; } = string.Empty;     // The number the code was sent to
        public int ResendInSeconds { get; set; }                    // Seconds left until a new code can be requested
        public bool HasActiveCode { get; set; }

        public bool IsComplete => EmailConfirmed && (!PhoneRequired || PhoneConfirmed);
    }

    // Doctor status for the Pending and Rejected pages
    public class DoctorStatusDto
    {
        public string FullName { get; set; } = string.Empty;
        public ApprovalStatus ApprovalStatus { get; set; }
        public string? RejectionReason { get; set; }
    }

    // The account resetting its password - data partially masked
    public class ResetAccountDto
    {
        public string UserId { get; set; } = string.Empty;
        public string MaskedEmail { get; set; } = string.Empty;
        public string MaskedPhone { get; set; } = string.Empty;
        public bool CanUseSms { get; set; } // SMS for patients and doctors only, with a verified number
    }

    // Settings page data (patient and doctor)
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