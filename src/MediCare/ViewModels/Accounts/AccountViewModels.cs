/*——- Omar Hussein ——-*/
using System.ComponentModel.DataAnnotations;
using MVC.BLL.DTOs.Accounts;
using MVC.DAL.Enums;

namespace MVC.PL.ViewModels.Accounts
{
    // نفس شروط الباسورد اللي في Figma: 8 حروف على الأقل + Capital و Small + رقم
    public static class PasswordRules
    {
        public const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,100}$";
        public const string Message = "Password must be at least 8 characters with upper and lower case letters and a number.";
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter your email.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = true;

        public string? ReturnUrl { get; set; }
    }

    public class RegisterPatientViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name is too long.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a valid email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a valid Egyptian mobile number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a valid date of birth.")]
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Select your gender.")]
        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "Enter your password.")]
        [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.Message)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm your password.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [MustBeTrue(ErrorMessage = "You must agree to the Terms and Privacy Policy.")]
        public bool AcceptTerms { get; set; }

        [Required(ErrorMessage = "Profile photo is required.")]
        public IFormFile? Photo { get; set; }
    }

    public class RegisterDoctorViewModel
    {
        // 1) Personal
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name is too long.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a valid email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a valid Egyptian mobile number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your password.")]
        [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.Message)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm your password.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Profile photo is required.")]
        public IFormFile? Photo { get; set; }

        // 2) Professional
        [Required(ErrorMessage = "Select a valid specialty.")]
        public Guid? SpecialtyId { get; set; }

        [Required(ErrorMessage = "License number is required.")]
        [StringLength(50, ErrorMessage = "License number is too long.")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a valid graduation year.")]
        [Range(1950, 2100, ErrorMessage = "Enter a valid graduation year.")]
        public int? GraduationYear { get; set; }

        [Required(ErrorMessage = "University is required.")]
        [StringLength(150, ErrorMessage = "University name is too long.")]
        public string University { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the year you started practicing.")]
        [Range(1950, 2100, ErrorMessage = "Enter a valid practice start year.")]
        public int? PracticeStartYear { get; set; }

        [Required(ErrorMessage = "Bio is required.")]
        [StringLength(1000, ErrorMessage = "Bio is too long.")]
        public string Bio { get; set; } = string.Empty;

        // 3) Documents
        [Required(ErrorMessage = "Medical license is required.")]
        public IFormFile? License { get; set; }

        [Required(ErrorMessage = "Graduation certificate is required.")]
        public IFormFile? GraduationCertificate { get; set; }


        // للعرض
        public List<SpecialtyDto> Specialties { get; set; } = new();
    }

    public class VerifyPhoneViewModel
    {
        [Required(ErrorMessage = "Enter the 6-digit code.")]
        [RegularExpression(@"^\s*\d{6}\s*$", ErrorMessage = "Enter the 6-digit code.")]
        public string Code { get; set; } = string.Empty;

        public VerificationStatusDto? Status { get; set; }
    }

    public class ChangeNumberViewModel
    {
        [Required(ErrorMessage = "Enter a valid Egyptian mobile number.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Enter your email.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;
    }

    public enum ResetMethod
    {
        Email,
        Sms
    }

    public class ResetOptionsViewModel
    {
        [Required]
        public string Ticket { get; set; } = string.Empty;

        public ResetMethod Method { get; set; } = ResetMethod.Email;

        public ResetAccountDto? Account { get; set; }
    }

    public class ResetCodeViewModel
    {
        [Required]
        public string Ticket { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the 6-digit code.")]
        [RegularExpression(@"^\s*\d{6}\s*$", ErrorMessage = "Enter the 6-digit code.")]
        public string Code { get; set; } = string.Empty;

        public string? MaskedPhone { get; set; } // للعرض بس
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your password.")]
        [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.Message)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm your password.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ResubmitDocumentsViewModel
    {
        [Required(ErrorMessage = "Medical license is required.")]
        public IFormFile? License { get; set; }

        [Required(ErrorMessage = "Graduation certificate is required.")]
        public IFormFile? GraduationCertificate { get; set; }

        public DoctorStatusDto? Status { get; set; }
    }

    // رسالة بسيطة بعد عملية (لينك اتبعت، الإيميل اتأكد...)
    public class AccountMessageViewModel
    {
        public string Icon { get; set; } = "mail";
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
        public bool DoctorPanel { get; set; }
    }
}

namespace MVC.PL.ViewModels.Accounts
{
    // صندوق رفع مستند في صفحات التسجيل والرفض
    public record UploadBoxModel(string Name, string Title, string Hint, bool Multiple, string? RequiredMessage = null);
}
/*————*/
