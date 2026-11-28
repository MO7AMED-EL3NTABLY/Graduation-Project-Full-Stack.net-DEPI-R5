/*——- Omar Hussein ——-*/
using MVC.BLL.Common;
using MVC.BLL.DTOs.Accounts;
using MVC.DAL.Enums;

namespace MVC.BLL.Interfaces
{
    // بيبني لينك فيه userId و token (الـ PL بيعمله بـ Url.Action عشان الـ BLL ميعرفش حاجة عن الـ Routes)
    public delegate string AccountLinkBuilder(string userId, string token);

    // بيبني لينك تأكيد تغيير الإيميل (فيه الإيميل الجديد كمان)
    public delegate string EmailChangeLinkBuilder(string userId, string newEmail, string token);

    // التسجيل وتسجيل الدخول وتأكيد الإيميل وحالة الدكتور
    public interface IAccountService
    {
        Task<List<SpecialtyDto>> GetActiveSpecialtiesAsync();

        // بيرجع الـ UserId لو نجح
        Task<ServiceResult<string>> RegisterPatientAsync(RegisterPatientDto dto, AccountLinkBuilder emailLink);

        Task<ServiceResult<string>> RegisterDoctorAsync(RegisterDoctorDto dto, AccountLinkBuilder emailLink);

        // بيدور بالإيميل أو برقم الموبايل المتأكد - بيرجع الـ UserId أو null
        Task<string?> FindUserIdByEmailAsync(string email);

        Task<LoginState> GetLoginStateAsync(string userId);

        Task<VerificationStatusDto?> GetVerificationStatusAsync(string userId);

        Task<ServiceResult> SendEmailConfirmationAsync(string userId, AccountLinkBuilder emailLink);

        Task<ServiceResult> ConfirmEmailAsync(string userId, string token);

        Task<DoctorStatusDto?> GetDoctorStatusAsync(string doctorId);

        // الدكتور المرفوض بيرفع رخصة وشهادة جداد فيرجع Pending
        Task<ServiceResult> ResubmitDoctorDocumentsAsync(string doctorId, FileUpload? license, FileUpload? graduationCertificate);
    }

    // كود تأكيد الموبايل (6 أرقام - 10 دقايق - 5 محاولات - كود جديد بعد 60 ثانية)
    public interface IPhoneVerificationService
    {
        // بيبعت كود للرقم المطلوب. الرقم الحالي مش بيتغير غير لما الكود يتأكد
        Task<ServiceResult> SendCodeAsync(string userId, string targetPhoneNumber);

        // بيبعت كود تاني لنفس الرقم اللي عليه الكود الحالي
        Task<ServiceResult> ResendCodeAsync(string userId);

        Task<ServiceResult> VerifyCodeAsync(string userId, string code);

        // الرقم اللي عليه كود شغال (لو فيه) وفاضل كام ثانية على إعادة الإرسال
        Task<(string? TargetPhoneNumber, int ResendInSeconds)> GetActiveCodeInfoAsync(string userId);
    }

    // نسيت الباسورد: لينك بالإيميل (ساعة) أو كود SMS (10 دقايق)
    public interface IPasswordResetService
    {
        Task<ResetAccountDto?> FindAccountAsync(string email);

        Task<ServiceResult> SendEmailLinkAsync(string userId, AccountLinkBuilder resetLink);

        Task<ServiceResult> SendSmsCodeAsync(string userId);

        // لو الكود صح بيرجع Reset token يتستخدم في ResetPasswordAsync
        Task<ServiceResult<string>> VerifySmsCodeAsync(string userId, string code);

        Task<ServiceResult> ResetPasswordAsync(string userId, string token, string newPassword);
    }

    // صفحة Settings: الاسم والصورة والإيميل والموبايل والباسورد
    public interface IAccountSettingsService
    {
        Task<AccountSettingsDto?> GetAsync(string userId);

        // المريض بيعدل اسمه وصورته من Settings (الدكتور من Professional profile)
        Task<ServiceResult> UpdatePatientProfileAsync(string userId, string fullName, FileUpload? newPhoto);

        // الإيميل الحالي بيفضل شغال لحد ما الجديد يتأكد من اللينك
        Task<ServiceResult> RequestEmailChangeAsync(string userId, string newEmail, EmailChangeLinkBuilder link);

        Task<ServiceResult> ConfirmEmailChangeAsync(string userId, string newEmail, string token);

        // الرقم الحالي بيفضل شغال لحد ما الجديد يتأكد بالكود
        Task<ServiceResult> RequestPhoneChangeAsync(string userId, string newPhoneNumber);

        Task<ServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }

    // صفحة Professional profile بتاعة الدكتور
    public interface IDoctorProfileService
    {
        Task<DoctorProfileDto?> GetAsync(string doctorId);

        Task<ServiceResult> UpdateBasicInfoAsync(string doctorId, string fullName, int practiceStartYear, string bio, FileUpload? newPhoto);

        // طلب تغيير حقل محمي (تخصص / رخصة / جامعة / سنة تخرج) - محتاج موافقة الأدمن
        Task<ServiceResult> RequestChangeAsync(string doctorId, ProfileChangeRequestType type, string requestedValue);

        Task<ServiceResult> UploadDocumentAsync(string doctorId, DoctorDocumentType type, FileUpload? file);

        Task<ServiceResult> AddSurgeryAsync(string doctorId, string surgeryName, string? description);

        Task<ServiceResult> RemoveSurgeryAsync(string doctorId, Guid surgeryId);

        Task<ServiceResult> AddWorkExperienceAsync(string doctorId, string organization, string position, int fromYear, int? toYear);

        Task<ServiceResult> RemoveWorkExperienceAsync(string doctorId, Guid workExperienceId);
    }

    // مراجعة الأدمن للدكاترة ومستنداتهم
    public interface IDoctorApprovalService
    {
        Task<ApprovalCountsDto> GetCountsAsync();

        Task<List<ApprovalListItemDto>> GetListAsync(ApprovalTab tab);

        Task<DoctorReviewDto?> GetDoctorReviewAsync(string doctorId);

        Task<ReplacementReviewDto?> GetReplacementReviewAsync(Guid documentId);

        // قرار واحد على الرخصة والشهادة مع بعض + حالة الدكتور
        Task<ServiceResult> DecideInitialAsync(string adminId, string doctorId, bool approve, string? rejectionReason);

        // قرار على المستند البديل بس - حالة الدكتور مش بتتغير
        Task<ServiceResult> DecideReplacementAsync(string adminId, Guid documentId, bool approve, string? rejectionReason);
    }

    // مراجعة الأدمن لطلبات تعديل البيانات المحمية
    public interface IProfileChangeRequestService
    {
        Task<ChangeRequestCountsDto> GetCountsAsync();

        Task<List<ChangeRequestRowDto>> GetListAsync(ApprovalStatus status);

        Task<ServiceResult> ApproveAsync(string adminId, Guid requestId);

        Task<ServiceResult> RejectAsync(string adminId, Guid requestId, string? reason);
    }

    // تحميل الملفات المحمية بعد التأكد من الصلاحية
    public interface IFileAccessService
    {
        Task<StoredFile?> GetUserPhotoAsync(string? requesterId, bool requesterIsAdmin, string targetUserId);

        Task<StoredFile?> GetDoctorDocumentAsync(string? requesterId, bool requesterIsAdmin, Guid documentId);
    }
}
/*————*/
