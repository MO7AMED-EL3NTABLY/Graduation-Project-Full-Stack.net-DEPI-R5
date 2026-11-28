/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC.BLL.Common;
using MVC.BLL.DTOs.Accounts;
using MVC.BLL.Interfaces;
using MVC.DAL.Constants;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Entities.Accounts;
using MVC.DAL.Enums;

namespace MVC.BLL.Services.Accounts
{
    // التسجيل وتسجيل الدخول وتأكيد الإيميل وحالة موافقة الدكتور
    public class AccountService : IAccountService
    {
        public const string PhotosFolder = "profile-photos";
        public const string DocumentsFolder = "doctor-documents";

        private readonly MediCareDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorage _files;
        private readonly IEmailSender _email;
        private readonly IPhoneVerificationService _phoneVerification;

        public AccountService(
            MediCareDbContext db,
            UserManager<ApplicationUser> userManager,
            IFileStorage files,
            IEmailSender email,
            IPhoneVerificationService phoneVerification)
        {
            _db = db;
            _userManager = userManager;
            _files = files;
            _email = email;
            _phoneVerification = phoneVerification;
        }

        public Task<List<SpecialtyDto>> GetActiveSpecialtiesAsync() =>
            _db.Specialties
               .Where(s => s.IsActive)
               .OrderBy(s => s.NameEn)
               .Select(s => new SpecialtyDto { SpecialtyId = s.SpecialtyId, NameEn = s.NameEn, NameAr = s.NameAr })
               .ToListAsync();

        // ---------------- Registration ----------------

        public async Task<ServiceResult<string>> RegisterPatientAsync(RegisterPatientDto dto, AccountLinkBuilder emailLink)
        {
            var errors = new List<string>();
            var phone = await ValidateNewAccountAsync(dto.FullName, dto.Email, dto.PhoneNumber, errors);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dto.DateOfBirth > today || dto.DateOfBirth < today.AddYears(-120))
                errors.Add("Enter a valid date of birth.");

            AddIfError(errors, FileRules.ValidatePhoto(dto.Photo));
            if (errors.Count > 0)
                return ServiceResult<string>.Fail(errors);

            var savedKeys = new List<string>();
            string userId;
            try
            {
                var photoKey = await _files.SaveAsync(dto.Photo!, PhotosFolder);
                savedKeys.Add(photoKey);

                // الحساب والـ Role والبروفايل في transaction واحدة - يا الكل يتحفظ يا ولا حاجة
                await using var transaction = await _db.Database.BeginTransactionAsync();

                var user = NewUser(dto.FullName, dto.Email, phone, photoKey);
                var created = await CreateUserWithRoleAsync(user, dto.Password, AppRoles.Patient);
                if (!created.Succeeded)
                {
                    DeleteFiles(savedKeys);
                    return ServiceResult<string>.Fail(created.Errors);
                }

                _db.PatientProfiles.Add(new PatientProfile
                {
                    PatientId = user.Id,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    ShareMedicalProfile = false // المشاركة بتبدأ مقفولة
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                userId = user.Id;
            }
            catch
            {
                DeleteFiles(savedKeys);
                throw;
            }

            await SendVerificationMessagesAsync(userId, phone, emailLink);
            return ServiceResult<string>.Success(userId);
        }

        public async Task<ServiceResult<string>> RegisterDoctorAsync(RegisterDoctorDto dto, AccountLinkBuilder emailLink)
        {
            var errors = new List<string>();
            var phone = await ValidateNewAccountAsync(dto.FullName, dto.Email, dto.PhoneNumber, errors);

            var specialtyOk = await _db.Specialties.AnyAsync(s => s.SpecialtyId == dto.SpecialtyId && s.IsActive);
            if (!specialtyOk)
                errors.Add("Select a valid specialty.");
            ValidateProfessionalInfo(dto.LicenseNumber, dto.University, dto.GraduationYear, dto.PracticeStartYear, dto.Bio, errors);

            AddIfError(errors, FileRules.ValidatePhoto(dto.Photo));
            if (dto.License is null || dto.License.Length == 0)
                errors.Add("Medical license is required.");
            else
                AddIfError(errors, FileRules.ValidateDocument(dto.License));
            if (dto.GraduationCertificate is null || dto.GraduationCertificate.Length == 0)
                errors.Add("Graduation certificate is required.");
            else
                AddIfError(errors, FileRules.ValidateDocument(dto.GraduationCertificate));

            if (errors.Count > 0)
                return ServiceResult<string>.Fail(errors.Distinct());

            var savedKeys = new List<string>();
            string userId;
            try
            {
                var photoKey = await _files.SaveAsync(dto.Photo!, PhotosFolder);
                savedKeys.Add(photoKey);

                var documents = new List<(DoctorDocumentType Type, FileUpload File, string Key)>
                {
                    (DoctorDocumentType.License, dto.License!, await SaveTracked(dto.License!, savedKeys)),
                    (DoctorDocumentType.GraduationCertificate, dto.GraduationCertificate!, await SaveTracked(dto.GraduationCertificate!, savedKeys))
                };

                await using var transaction = await _db.Database.BeginTransactionAsync();

                var user = NewUser(dto.FullName, dto.Email, phone, photoKey);
                var created = await CreateUserWithRoleAsync(user, dto.Password, AppRoles.Doctor);
                if (!created.Succeeded)
                {
                    DeleteFiles(savedKeys);
                    return ServiceResult<string>.Fail(created.Errors);
                }

                _db.DoctorProfiles.Add(new DoctorProfile
                {
                    DoctorId = user.Id,
                    SpecialtyId = dto.SpecialtyId,
                    LicenseNumber = dto.LicenseNumber.Trim(),
                    University = dto.University.Trim(),
                    GraduationYear = dto.GraduationYear,
                    PracticeStartYear = dto.PracticeStartYear,
                    Bio = dto.Bio.Trim(),
                    ApprovalStatus = ApprovalStatus.Pending // مش بيظهر في البحث لحد موافقة الأدمن
                });

                var now = DateTimeOffset.UtcNow;
                foreach (var doc in documents)
                    _db.DoctorDocuments.Add(NewPendingDocument(user.Id, doc.Type, doc.File, doc.Key, now));

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                userId = user.Id;
            }
            catch
            {
                DeleteFiles(savedKeys);
                throw;
            }

            await SendVerificationMessagesAsync(userId, phone, emailLink);
            return ServiceResult<string>.Success(userId);
        }

        // ---------------- Login & verification ----------------

        // الدخول بالإيميل بس (Spec §5.1: email/password form)
        public async Task<string?> FindUserIdByEmailAsync(string email)
        {
            var input = (email ?? string.Empty).Trim();
            if (input.Length == 0 || !ContactFormat.LooksLikeEmail(input))
                return null;

            return (await _userManager.FindByEmailAsync(input))?.Id;
        }

        public async Task<LoginState> GetLoginStateAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || !user.IsActive)
                return LoginState.Inactive;

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (role is null)
                return LoginState.Inactive;

            var isAdmin = role == AppRoles.Admin;
            if (!user.EmailConfirmed || (!isAdmin && !user.PhoneNumberConfirmed))
                return LoginState.NeedsVerification;

            if (isAdmin)
                return LoginState.Admin;
            if (role == AppRoles.Patient)
                return LoginState.Patient;

            var status = await _db.DoctorProfiles.Where(d => d.DoctorId == userId).Select(d => (ApprovalStatus?)d.ApprovalStatus).FirstOrDefaultAsync();
            return status switch
            {
                ApprovalStatus.Approved => LoginState.DoctorApproved,
                ApprovalStatus.Rejected => LoginState.DoctorRejected,
                ApprovalStatus.Pending => LoginState.DoctorPending,
                _ => LoginState.Inactive
            };
        }

        public async Task<VerificationStatusDto?> GetVerificationStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null;

            var isAdmin = await _userManager.IsInRoleAsync(user, AppRoles.Admin);
            var (target, resendIn) = await _phoneVerification.GetActiveCodeInfoAsync(userId);

            return new VerificationStatusDto
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                PhoneRequired = !isAdmin,
                PhoneConfirmed = user.PhoneNumberConfirmed,
                MaskedPhone = ContactFormat.MaskPhone(target ?? user.PhoneNumber),
                ResendInSeconds = resendIn,
                HasActiveCode = target is not null
            };
        }

        public async Task<ServiceResult> SendEmailConfirmationAsync(string userId, AccountLinkBuilder emailLink)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Fail("Account not found.");
            if (user.EmailConfirmed)
                return ServiceResult.Success();

            // Identity token - صالح 24 ساعة (الافتراضي بتاع DataProtectorTokenProvider)
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = emailLink(user.Id, token);
            await _email.SendAsync(user.Email!, "Confirm your MediCare email",
                EmailTemplates.ActionEmail(user.FullName,
                    "Confirm your email",
                    "Thanks for joining MediCare. Please confirm your email address to finish setting up your account.",
                    "Confirm email", link,
                    "This link is valid for 24 hours."));
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || string.IsNullOrEmpty(token))
                return ServiceResult.Fail("This confirmation link is invalid or has expired.");
            if (user.EmailConfirmed)
                return ServiceResult.Success();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded
                ? ServiceResult.Success()
                : ServiceResult.Fail("This confirmation link is invalid or has expired.");
        }

        // ---------------- Doctor approval status ----------------

        public async Task<DoctorStatusDto?> GetDoctorStatusAsync(string doctorId)
        {
            var doctor = await _db.DoctorProfiles
                .Where(d => d.DoctorId == doctorId)
                .Select(d => new { d.User.FullName, d.ApprovalStatus })
                .FirstOrDefaultAsync();
            if (doctor is null)
                return null;

            string? reason = null;
            if (doctor.ApprovalStatus == ApprovalStatus.Rejected)
            {
                reason = await _db.DoctorDocuments
                    .Where(x => x.DoctorId == doctorId && x.ApprovalStatus == ApprovalStatus.Rejected)
                    .OrderByDescending(x => x.ReviewedAtUtc)
                    .Select(x => x.RejectionReason)
                    .FirstOrDefaultAsync();
            }

            return new DoctorStatusDto { FullName = doctor.FullName, ApprovalStatus = doctor.ApprovalStatus, RejectionReason = reason };
        }

        public async Task<ServiceResult> ResubmitDoctorDocumentsAsync(string doctorId, FileUpload? license, FileUpload? graduationCertificate)
        {
            var profile = await _db.DoctorProfiles.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (profile is null)
                return ServiceResult.Fail("Doctor profile not found.");
            if (profile.ApprovalStatus != ApprovalStatus.Rejected)
                return ServiceResult.Fail("Only a rejected application can be resubmitted.");

            var errors = new List<string>();
            if (license is null || license.Length == 0)
                errors.Add("Medical license is required.");
            else
                AddIfError(errors, FileRules.ValidateDocument(license));
            if (graduationCertificate is null || graduationCertificate.Length == 0)
                errors.Add("Graduation certificate is required.");
            else
                AddIfError(errors, FileRules.ValidateDocument(graduationCertificate));
            if (errors.Count > 0)
                return ServiceResult.Fail(errors.Distinct());

            var savedKeys = new List<string>();
            try
            {
                var licenseKey = await SaveTracked(license!, savedKeys);
                var certificateKey = await SaveTracked(graduationCertificate!, savedKeys);
                var now = DateTimeOffset.UtcNow;

                // المستندات المرفوضة بتفضل بسببها، والجداد بيرجّعوا الدكتور Pending لمراجعة أولى جديدة
                await using var transaction = await _db.Database.BeginTransactionAsync();
                _db.DoctorDocuments.Add(NewPendingDocument(doctorId, DoctorDocumentType.License, license!, licenseKey, now));
                _db.DoctorDocuments.Add(NewPendingDocument(doctorId, DoctorDocumentType.GraduationCertificate, graduationCertificate!, certificateKey, now));
                profile.ApprovalStatus = ApprovalStatus.Pending;
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                DeleteFiles(savedKeys);
                throw;
            }

            return ServiceResult.Success();
        }

        // ---------------- Helpers ----------------

        // بيتأكد من الاسم والإيميل والموبايل، وبيرجع الرقم بعد التوحيد
        private async Task<string> ValidateNewAccountAsync(string fullName, string email, string phoneNumber, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                errors.Add("Full name is required.");

            if (string.IsNullOrWhiteSpace(email) || !ContactFormat.LooksLikeEmail(email))
                errors.Add("Enter a valid email address.");
            else if (await _userManager.FindByEmailAsync(email.Trim()) is not null)
                errors.Add("An account with this email already exists.");

            if (!ContactFormat.TryNormalizePhone(phoneNumber, out var phone))
            {
                errors.Add("Enter a valid Egyptian mobile number.");
                return string.Empty;
            }

            if (await _db.Users.AnyAsync(u => u.PhoneNumber == phone && u.PhoneNumberConfirmed))
                errors.Add("This phone number is already used by another account.");

            return phone;
        }

        internal static void ValidateProfessionalInfo(string licenseNumber, string university, int graduationYear, int practiceStartYear, string bio, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(licenseNumber))
                errors.Add("License number is required.");
            if (string.IsNullOrWhiteSpace(university))
                errors.Add("University is required.");
            if (graduationYear < 1950 || graduationYear > DateTime.UtcNow.Year)
                errors.Add("Enter a valid graduation year.");
            else if (ValidatePracticeStartYear(practiceStartYear, graduationYear) is { } startYearError)
                errors.Add(startYearError);
            if (string.IsNullOrWhiteSpace(bio))
                errors.Add("Bio is required.");
        }

        // D-03: سنة بداية الممارسة: مش قبل سنة التخرج ومش بعد السنة الحالية
        internal static string? ValidatePracticeStartYear(int practiceStartYear, int graduationYear)
        {
            if (practiceStartYear > DateTime.UtcNow.Year)
                return "Practice start year can't be in the future.";
            if (practiceStartYear < graduationYear)
                return "Practice start year can't be before your graduation year.";
            return null;
        }

        private static ApplicationUser NewUser(string fullName, string email, string phone, string photoKey) => new()
        {
            UserName = email.Trim(),
            Email = email.Trim(),
            FullName = fullName.Trim(),
            PhoneNumber = phone,            // بيتأكد بالكود بعد التسجيل
            PhoneNumberConfirmed = false,
            ProfilePhotoStorageKey = photoKey,
            IsActive = true
        };

        private async Task<ServiceResult> CreateUserWithRoleAsync(ApplicationUser user, string password, string role)
        {
            var create = await _userManager.CreateAsync(user, password);
            if (!create.Succeeded)
                return ServiceResult.Fail(create.Errors.Select(e => e.Description));

            var addRole = await _userManager.AddToRoleAsync(user, role);
            if (!addRole.Succeeded)
                return ServiceResult.Fail(addRole.Errors.Select(e => e.Description));

            return ServiceResult.Success();
        }

        internal static DoctorDocument NewPendingDocument(string doctorId, DoctorDocumentType type, FileUpload file, string key, DateTimeOffset now) => new()
        {
            DoctorId = doctorId,
            DocumentType = type,
            StorageKey = key,
            OriginalFileName = Path.GetFileName(file.FileName),
            ContentType = FileRules.ContentTypeFor(file.FileName),
            FileSizeBytes = file.Length,
            ApprovalStatus = ApprovalStatus.Pending,
            IsCurrent = false,
            UploadedAtUtc = now
        };

        private async Task<string> SaveTracked(FileUpload file, List<string> savedKeys)
        {
            var key = await _files.SaveAsync(file, DocumentsFolder);
            savedKeys.Add(key);
            return key;
        }

        private async Task SendVerificationMessagesAsync(string userId, string phone, AccountLinkBuilder emailLink)
        {
            await SendEmailConfirmationAsync(userId, emailLink);
            await _phoneVerification.SendCodeAsync(userId, phone);
        }

        private void DeleteFiles(IEnumerable<string> keys)
        {
            foreach (var key in keys)
                _files.Delete(key);
        }

        private static void AddIfError(List<string> errors, string? error)
        {
            if (error is not null)
                errors.Add(error);
        }
    }
}
/*————*/
