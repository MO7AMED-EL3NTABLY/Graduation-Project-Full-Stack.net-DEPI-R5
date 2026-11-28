using System.ComponentModel.DataAnnotations;
using MVC.BLL.DTOs.Accounts;
using MVC.DAL.Enums;

namespace MVC.PL.ViewModels.Accounts
{
    // Settings page (patient and doctor) - separate forms so each one has its own validation
    public class SettingsPageViewModel
    {
        public AccountSettingsDto Account { get; set; } = new();
        public bool IsDoctor { get; set; }

        // A new number waiting for the code (if any)
        public string? PendingPhone { get; set; }
        public int ResendInSeconds { get; set; }

        public ProfileFormViewModel Profile { get; set; } = new();
        public string? OpenModal { get; set; } // email / phone / code / password
        public ChangeEmailViewModel ChangeEmail { get; set; } = new();
        public ChangePhoneViewModel ChangePhone { get; set; } = new();
        public VerifyPhoneViewModel VerifyCode { get; set; } = new();
        public ChangePasswordViewModel ChangePassword { get; set; } = new();
    }

    public class ProfileFormViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name is too long.")]
        public string FullName { get; set; } = string.Empty;

        public IFormFile? Photo { get; set; }
    }

    public class ChangeEmailViewModel
    {
        [Required(ErrorMessage = "Enter a valid email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(256)]
        public string NewEmail { get; set; } = string.Empty;
    }

    public class ChangePhoneViewModel
    {
        [Required(ErrorMessage = "Enter a valid Egyptian mobile number.")]
        public string NewPhoneNumber { get; set; } = string.Empty;
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Enter your current password.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your password.")]
        [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.Message)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm your password.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ===== Doctor professional profile =====

    public class DoctorProfilePageViewModel
    {
        public DoctorProfileDto Profile { get; set; } = new();
        public string? OpenModal { get; set; } // change-<type> / upload / surgery / experience
        public BasicInfoViewModel BasicInfo { get; set; } = new();
        public RequestChangeViewModel RequestChange { get; set; } = new();
        public UploadDocumentViewModel Upload { get; set; } = new();
        public AddSurgeryViewModel Surgery { get; set; } = new();
        public AddWorkExperienceViewModel Experience { get; set; } = new();
    }

    public class BasicInfoViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name is too long.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the year you started practicing.")]
        [Range(1950, 2100, ErrorMessage = "Enter a valid practice start year.")]
        public int? PracticeStartYear { get; set; }

        [Required(ErrorMessage = "Bio is required.")]
        [StringLength(1000, ErrorMessage = "Bio is too long.")]
        public string Bio { get; set; } = string.Empty;

        public IFormFile? Photo { get; set; }
    }

    public class RequestChangeViewModel
    {
        public ProfileChangeRequestType RequestType { get; set; }

        [Required(ErrorMessage = "Enter the new value.")]
        [StringLength(150, ErrorMessage = "The value is too long.")]
        public string RequestedValue { get; set; } = string.Empty;
    }

    public class UploadDocumentViewModel
    {
        public DoctorDocumentType DocumentType { get; set; } = DoctorDocumentType.License;

        [Required(ErrorMessage = "Choose a file to upload.")]
        public IFormFile? File { get; set; }
    }

    public class AddSurgeryViewModel
    {
        [Required(ErrorMessage = "Surgery name is required.")]
        [StringLength(150, ErrorMessage = "Surgery name is too long.")]
        public string SurgeryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description is too long.")]
        public string? Description { get; set; }
    }

    public class AddWorkExperienceViewModel
    {
        [Required(ErrorMessage = "Organization is required.")]
        [StringLength(150, ErrorMessage = "Organization is too long.")]
        public string Organization { get; set; } = string.Empty;

        [Required(ErrorMessage = "Position is required.")]
        [StringLength(100, ErrorMessage = "Position is too long.")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a valid start year.")]
        [Range(1950, 2100, ErrorMessage = "Enter a valid start year.")]
        public int? FromYear { get; set; }

        [Range(1950, 2100, ErrorMessage = "End year must be between the start year and this year.")]
        public int? ToYear { get; set; }

        public bool CurrentlyWorking { get; set; }
    }

    // ===== Admin =====

    public class DoctorApprovalsPageViewModel
    {
        public ApprovalTab Tab { get; set; }
        public ApprovalCountsDto Counts { get; set; } = new();
        public List<ApprovalListItemDto> Items { get; set; } = new();
        public string? SelectedDoctorId { get; set; }
        public Guid? SelectedDocumentId { get; set; }
        public DoctorReviewDto? Doctor { get; set; }
        public ReplacementReviewDto? Replacement { get; set; }
    }

    public class ApprovalDecisionViewModel
    {
        public string? DoctorId { get; set; }
        public Guid? DocumentId { get; set; }
        public bool Approve { get; set; }

        [StringLength(500, ErrorMessage = "The reason is too long.")]
        public string? RejectionReason { get; set; }
    }

    public class ChangeRequestsPageViewModel
    {
        public ApprovalStatus Status { get; set; }
        public ChangeRequestCountsDto Counts { get; set; } = new();
        public List<ChangeRequestRowDto> Rows { get; set; } = new();
    }
}
