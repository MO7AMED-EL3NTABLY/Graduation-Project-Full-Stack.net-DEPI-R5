using MVC.BLL.Common;
using MVC.BLL.DTOs.Accounts;
using MVC.DAL.Enums;

namespace MVC.BLL.Interfaces
{
    // Builds a link with userId and token (the PL builds it with Url.Action so the BLL knows nothing about routes)
    public delegate string AccountLinkBuilder(string userId, string token);

    // Builds the email-change confirmation link (also includes the new email)
    public delegate string EmailChangeLinkBuilder(string userId, string newEmail, string token);

    // Registration, sign-in, email confirmation and doctor status
    public interface IAccountService
    {
        Task<List<SpecialtyDto>> GetActiveSpecialtiesAsync();

        // Returns the UserId on success
        Task<ServiceResult<string>> RegisterPatientAsync(RegisterPatientDto dto, AccountLinkBuilder emailLink);

        Task<ServiceResult<string>> RegisterDoctorAsync(RegisterDoctorDto dto, AccountLinkBuilder emailLink);

        // Finds the account by email - returns the UserId or null
        Task<string?> FindUserIdByEmailAsync(string email);

        Task<LoginState> GetLoginStateAsync(string userId);

        Task<VerificationStatusDto?> GetVerificationStatusAsync(string userId);

        Task<ServiceResult> SendEmailConfirmationAsync(string userId, AccountLinkBuilder emailLink);

        Task<ServiceResult> ConfirmEmailAsync(string userId, string token);

        Task<DoctorStatusDto?> GetDoctorStatusAsync(string doctorId);

        // A rejected doctor uploads a new license and certificate and goes back to Pending
        Task<ServiceResult> ResubmitDoctorDocumentsAsync(string doctorId, FileUpload? license, FileUpload? graduationCertificate);
    }

    // Phone verification code (6 digits - 10 minutes - 5 attempts - new code after 60 seconds)
    public interface IPhoneVerificationService
    {
        // Sends a code to the given number. The current number doesn't change until the code is verified
        Task<ServiceResult> SendCodeAsync(string userId, string targetPhoneNumber);

        // Sends another code to the same number as the current code
        Task<ServiceResult> ResendCodeAsync(string userId);

        Task<ServiceResult> VerifyCodeAsync(string userId, string code);

        // The number with an active code (if any) and how many seconds are left before resending
        Task<(string? TargetPhoneNumber, int ResendInSeconds)> GetActiveCodeInfoAsync(string userId);
    }

    // Forgot password: email link (1 hour) or SMS code (10 minutes)
    public interface IPasswordResetService
    {
        Task<ResetAccountDto?> FindAccountAsync(string email);

        Task<ServiceResult> SendEmailLinkAsync(string userId, AccountLinkBuilder resetLink);

        Task<ServiceResult> SendSmsCodeAsync(string userId);

        // If the code is correct, returns a reset token used in ResetPasswordAsync
        Task<ServiceResult<string>> VerifySmsCodeAsync(string userId, string code);

        Task<ServiceResult> ResetPasswordAsync(string userId, string token, string newPassword);
    }

    // Settings page: name, photo, email, phone and password
    public interface IAccountSettingsService
    {
        Task<AccountSettingsDto?> GetAsync(string userId);

        // The patient edits name and photo from Settings (the doctor from Professional profile)
        Task<ServiceResult> UpdatePatientProfileAsync(string userId, string fullName, FileUpload? newPhoto);

        // The current email stays active until the new one is confirmed from the link
        Task<ServiceResult> RequestEmailChangeAsync(string userId, string newEmail, EmailChangeLinkBuilder link);

        Task<ServiceResult> ConfirmEmailChangeAsync(string userId, string newEmail, string token);

        // The current number stays active until the new one is verified with the code
        Task<ServiceResult> RequestPhoneChangeAsync(string userId, string newPhoneNumber);

        Task<ServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }

    // The doctor's Professional profile page
    public interface IDoctorProfileService
    {
        Task<DoctorProfileDto?> GetAsync(string doctorId);

        Task<ServiceResult> UpdateBasicInfoAsync(string doctorId, string fullName, int practiceStartYear, string bio, FileUpload? newPhoto);

        // Request to change a protected field (specialty / license / university / graduation year) - needs admin approval
        Task<ServiceResult> RequestChangeAsync(string doctorId, ProfileChangeRequestType type, string requestedValue);

        Task<ServiceResult> UploadDocumentAsync(string doctorId, DoctorDocumentType type, FileUpload? file);

        Task<ServiceResult> AddSurgeryAsync(string doctorId, string surgeryName, string? description);

        Task<ServiceResult> RemoveSurgeryAsync(string doctorId, Guid surgeryId);

        Task<ServiceResult> AddWorkExperienceAsync(string doctorId, string organization, string position, int fromYear, int? toYear);

        Task<ServiceResult> RemoveWorkExperienceAsync(string doctorId, Guid workExperienceId);
    }

    // Admin review of doctors and their documents
    public interface IDoctorApprovalService
    {
        Task<ApprovalCountsDto> GetCountsAsync();

        Task<List<ApprovalListItemDto>> GetListAsync(ApprovalTab tab);

        Task<DoctorReviewDto?> GetDoctorReviewAsync(string doctorId);

        Task<ReplacementReviewDto?> GetReplacementReviewAsync(Guid documentId);

        // One decision on the license and certificate together + the doctor's status
        Task<ServiceResult> DecideInitialAsync(string adminId, string doctorId, bool approve, string? rejectionReason);

        // Decision on the replacement document only - the doctor's status doesn't change
        Task<ServiceResult> DecideReplacementAsync(string adminId, Guid documentId, bool approve, string? rejectionReason);
    }

    // Admin review of protected data change requests
    public interface IProfileChangeRequestService
    {
        Task<ChangeRequestCountsDto> GetCountsAsync();

        Task<List<ChangeRequestRowDto>> GetListAsync(ApprovalStatus status);

        Task<ServiceResult> ApproveAsync(string adminId, Guid requestId);

        Task<ServiceResult> RejectAsync(string adminId, Guid requestId, string? reason);
    }

    // Downloads protected files after checking authorization
    public interface IFileAccessService
    {
        Task<StoredFile?> GetUserPhotoAsync(string? requesterId, bool requesterIsAdmin, string targetUserId);

        Task<StoredFile?> GetDoctorDocumentAsync(string? requesterId, bool requesterIsAdmin, Guid documentId);
    }
}
