/*——- Omar Hussein ——-*/
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC.BLL.Common;
using MVC.BLL.DTOs.Accounts;
using MVC.BLL.Interfaces;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Entities.Accounts;
using MVC.DAL.Enums;

namespace MVC.BLL.Services.Accounts
{
    // صفحة Professional profile: البيانات العادية بتتعدل علطول، والمحمية بطلب للأدمن
    public class DoctorProfileService : IDoctorProfileService
    {
        private readonly MediCareDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorage _files;

        public DoctorProfileService(MediCareDbContext db, UserManager<ApplicationUser> userManager, IFileStorage files)
        {
            _db = db;
            _userManager = userManager;
            _files = files;
        }

        public async Task<DoctorProfileDto?> GetAsync(string doctorId)
        {
            var doctor = await _db.DoctorProfiles
                .AsNoTracking()
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Include(d => d.Surgeries)
                .Include(d => d.WorkExperiences)
                .Include(d => d.Documents)
                .Include(d => d.ChangeRequests.Where(r => r.Status == ApprovalStatus.Pending))
                    .ThenInclude(r => r.RequestedSpecialty)
                .AsSplitQuery()
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (doctor is null)
                return null;

            return new DoctorProfileDto
            {
                DoctorId = doctor.DoctorId,
                FullName = doctor.User.FullName,
                HasPhoto = !string.IsNullOrEmpty(doctor.User.ProfilePhotoStorageKey),
                PracticeStartYear = doctor.PracticeStartYear,
                YearsOfExperience = Experience.Years(doctor.PracticeStartYear),
                Bio = doctor.Bio,
                ApprovalStatus = doctor.ApprovalStatus,
                Specialty = ToDto(doctor.Specialty),
                LicenseNumber = doctor.LicenseNumber,
                University = doctor.University,
                GraduationYear = doctor.GraduationYear,
                PendingChanges = doctor.ChangeRequests.Select(r => new PendingChangeDto
                {
                    RequestType = r.RequestType,
                    RequestedValue = r.RequestType switch
                    {
                        ProfileChangeRequestType.LicenseNumber => r.RequestedLicenseNumber,
                        ProfileChangeRequestType.University => r.RequestedUniversity,
                        ProfileChangeRequestType.GraduationYear => r.RequestedGraduationYear?.ToString(),
                        _ => null
                    },
                    RequestedSpecialty = r.RequestedSpecialty is null ? null : ToDto(r.RequestedSpecialty)
                }).ToList(),
                // المستندات المعتمدة القديمة اللي اتبدلت مش بتظهر
                Documents = doctor.Documents
                    .Where(x => !(x.ApprovalStatus == ApprovalStatus.Approved && !x.IsCurrent))
                    .OrderBy(x => x.DocumentType)
                    .ThenByDescending(x => x.UploadedAtUtc)
                    .Select(ToDto)
                    .ToList(),
                Surgeries = doctor.Surgeries
                    .OrderBy(s => s.SurgeryName)
                    .Select(s => new DoctorSurgeryDto { DoctorSurgeryId = s.DoctorSurgeryId, SurgeryName = s.SurgeryName, Description = s.Description })
                    .ToList(),
                WorkExperiences = doctor.WorkExperiences
                    .OrderBy(w => w.ToYear.HasValue)          // الحالي (ToYear = null) الأول
                    .ThenByDescending(w => w.ToYear)
                    .ThenByDescending(w => w.FromYear)
                    .Select(w => new DoctorWorkExperienceDto
                    {
                        DoctorWorkExperienceId = w.DoctorWorkExperienceId,
                        Organization = w.Organization,
                        Position = w.Position,
                        FromYear = w.FromYear,
                        ToYear = w.ToYear
                    })
                    .ToList(),
                ActiveSpecialties = await _db.Specialties
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.NameEn)
                    .Select(s => new SpecialtyDto { SpecialtyId = s.SpecialtyId, NameEn = s.NameEn, NameAr = s.NameAr })
                    .ToListAsync()
            };
        }

        public async Task<ServiceResult> UpdateBasicInfoAsync(string doctorId, string fullName, int practiceStartYear, string bio, FileUpload? newPhoto)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(fullName))
                errors.Add("Full name is required.");
            if (string.IsNullOrWhiteSpace(bio))
                errors.Add("Bio is required.");
            if (errors.Count > 0)
                return ServiceResult.Fail(errors);

            var profile = await _db.DoctorProfiles.Include(d => d.User).FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (profile is null)
                return ServiceResult.Fail("Doctor profile not found.");

            // D-03: سنة البداية مش قبل سنة التخرج المعتمدة ومش بعد السنة الحالية
            var startYearError = AccountService.ValidatePracticeStartYear(practiceStartYear, profile.GraduationYear);
            if (startYearError is not null)
                return ServiceResult.Fail(startYearError);

            // البيانات العادية بتتعدل من غير موافقة الأدمن
            profile.PracticeStartYear = practiceStartYear;
            profile.Bio = bio.Trim();

            // UpdateAsync بيعمل SaveChanges فبيحفظ تعديل البروفايل كمان
            return await AccountSettingsService.UpdateNameAndPhotoAsync(_userManager, _files, profile.User, fullName, newPhoto);
        }

        public async Task<ServiceResult> RequestChangeAsync(string doctorId, ProfileChangeRequestType type, string requestedValue)
        {
            var profile = await _db.DoctorProfiles.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (profile is null)
                return ServiceResult.Fail("Doctor profile not found.");

            var hasPending = await _db.DoctorProfileChangeRequests
                .AnyAsync(r => r.DoctorId == doctorId && r.RequestType == type && r.Status == ApprovalStatus.Pending);
            if (hasPending)
                return ServiceResult.Fail("You already have a pending request for this field.");

            var value = (requestedValue ?? string.Empty).Trim();
            var request = new DoctorProfileChangeRequest
            {
                DoctorId = doctorId,
                RequestType = type,
                Status = ApprovalStatus.Pending,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            // كل طلب بيملا العمود اللي يقابل نوعه بس (CHECK في الداتابيز)
            switch (type)
            {
                case ProfileChangeRequestType.Specialty:
                    if (!Guid.TryParse(value, out var specialtyId)
                        || !await _db.Specialties.AnyAsync(s => s.SpecialtyId == specialtyId && s.IsActive))
                        return ServiceResult.Fail("Select a valid specialty.");
                    if (specialtyId == profile.SpecialtyId)
                        return ServiceResult.Fail("The requested value is the same as the current one.");
                    request.RequestedSpecialtyId = specialtyId;
                    break;

                case ProfileChangeRequestType.LicenseNumber:
                    if (value.Length == 0)
                        return ServiceResult.Fail("License number is required.");
                    if (value == profile.LicenseNumber)
                        return ServiceResult.Fail("The requested value is the same as the current one.");
                    request.RequestedLicenseNumber = value;
                    break;

                case ProfileChangeRequestType.University:
                    if (value.Length == 0)
                        return ServiceResult.Fail("University is required.");
                    if (value == profile.University)
                        return ServiceResult.Fail("The requested value is the same as the current one.");
                    request.RequestedUniversity = value;
                    break;

                case ProfileChangeRequestType.GraduationYear:
                    if (!int.TryParse(value, out var year) || year < 1950 || year > DateTime.UtcNow.Year)
                        return ServiceResult.Fail("Enter a valid graduation year.");
                    if (year == profile.GraduationYear)
                        return ServiceResult.Fail("The requested value is the same as the current one.");
                    request.RequestedGraduationYear = year;
                    break;

                default:
                    return ServiceResult.Fail("Unknown request type.");
            }

            _db.DoctorProfileChangeRequests.Add(request);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // الـ filtered unique index بيمنع طلبين Pending لنفس الحقل لو اتبعتوا في نفس اللحظة
                return ServiceResult.Fail("You already have a pending request for this field.");
            }

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UploadDocumentAsync(string doctorId, DoctorDocumentType type, FileUpload? file)
        {
            var fileError = FileRules.ValidateDocument(file);
            if (fileError is not null)
                return ServiceResult.Fail(fileError);

            var profile = await _db.DoctorProfiles.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (profile is null)
                return ServiceResult.Fail("Doctor profile not found.");

            // D-01: النوعين بس (License / GraduationCertificate)
            if (!Enum.IsDefined(type))
                return ServiceResult.Fail("Choose a valid document type.");

            // بعد الموافقة الأولى بس، وكبديل للمستند المعتمد الحالي
            if (profile.ApprovalStatus == ApprovalStatus.Pending)
                return ServiceResult.Fail("Your documents are already under review.");
            if (profile.ApprovalStatus == ApprovalStatus.Rejected)
                return ServiceResult.Fail("Upload new documents from your application status page.");

            var hasPending = await _db.DoctorDocuments
                .AnyAsync(x => x.DoctorId == doctorId && x.DocumentType == type && x.ApprovalStatus == ApprovalStatus.Pending);
            if (hasPending)
                return ServiceResult.Fail("You already have a replacement waiting for review.");

            Guid? replacesId = await _db.DoctorDocuments
                .Where(x => x.DoctorId == doctorId && x.DocumentType == type && x.ApprovalStatus == ApprovalStatus.Approved && x.IsCurrent)
                .Select(x => (Guid?)x.DoctorDocumentId)
                .FirstOrDefaultAsync();

            var key = await _files.SaveAsync(file!, AccountService.DocumentsFolder);
            try
            {
                // البديل بيفضل Pending لحد قرار الأدمن
                var document = AccountService.NewPendingDocument(doctorId, type, file!, key, DateTimeOffset.UtcNow);
                document.ReplacesDocumentId = replacesId;
                _db.DoctorDocuments.Add(document);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                _files.Delete(key);
                return ServiceResult.Fail("You already have a replacement waiting for review.");
            }
            catch
            {
                _files.Delete(key);
                throw;
            }

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> AddSurgeryAsync(string doctorId, string surgeryName, string? description)
        {
            if (string.IsNullOrWhiteSpace(surgeryName))
                return ServiceResult.Fail("Surgery name is required.");
            if (!await _db.DoctorProfiles.AnyAsync(d => d.DoctorId == doctorId))
                return ServiceResult.Fail("Doctor profile not found.");

            _db.DoctorSurgeries.Add(new DoctorSurgery
            {
                DoctorId = doctorId,
                SurgeryName = surgeryName.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
            });
            await _db.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RemoveSurgeryAsync(string doctorId, Guid surgeryId)
        {
            // بنتأكد إن العملية بتاعة نفس الدكتور (IDOR)
            var surgery = await _db.DoctorSurgeries.FirstOrDefaultAsync(s => s.DoctorSurgeryId == surgeryId && s.DoctorId == doctorId);
            if (surgery is null)
                return ServiceResult.Fail("Item not found.");

            _db.DoctorSurgeries.Remove(surgery);
            await _db.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> AddWorkExperienceAsync(string doctorId, string organization, string position, int fromYear, int? toYear)
        {
            var errors = new List<string>();
            var thisYear = DateTime.UtcNow.Year;
            if (string.IsNullOrWhiteSpace(organization))
                errors.Add("Organization is required.");
            if (string.IsNullOrWhiteSpace(position))
                errors.Add("Position is required.");
            if (fromYear < 1950 || fromYear > thisYear)
                errors.Add("Enter a valid start year.");
            if (toYear.HasValue && (toYear < fromYear || toYear > thisYear))
                errors.Add("End year must be between the start year and this year.");
            if (errors.Count > 0)
                return ServiceResult.Fail(errors);

            if (!await _db.DoctorProfiles.AnyAsync(d => d.DoctorId == doctorId))
                return ServiceResult.Fail("Doctor profile not found.");

            _db.DoctorWorkExperiences.Add(new DoctorWorkExperience
            {
                DoctorId = doctorId,
                Organization = organization.Trim(),
                Position = position.Trim(),
                FromYear = fromYear,
                ToYear = toYear // null = لسه شغال فيه
            });
            await _db.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RemoveWorkExperienceAsync(string doctorId, Guid workExperienceId)
        {
            var item = await _db.DoctorWorkExperiences.FirstOrDefaultAsync(w => w.DoctorWorkExperienceId == workExperienceId && w.DoctorId == doctorId);
            if (item is null)
                return ServiceResult.Fail("Item not found.");

            _db.DoctorWorkExperiences.Remove(item);
            await _db.SaveChangesAsync();
            return ServiceResult.Success();
        }

        internal static SpecialtyDto ToDto(Specialty s) => new() { SpecialtyId = s.SpecialtyId, NameEn = s.NameEn, NameAr = s.NameAr };

        internal static DoctorDocumentDto ToDto(DoctorDocument d) => new()
        {
            DoctorDocumentId = d.DoctorDocumentId,
            DocumentType = d.DocumentType,
            ApprovalStatus = d.ApprovalStatus,
            IsCurrent = d.IsCurrent,
            IsReplacement = d.ReplacesDocumentId.HasValue,
            OriginalFileName = d.OriginalFileName,
            UploadedAtUtc = d.UploadedAtUtc,
            ReviewedAtUtc = d.ReviewedAtUtc,
            RejectionReason = d.RejectionReason
        };
    }
}
/*————*/
