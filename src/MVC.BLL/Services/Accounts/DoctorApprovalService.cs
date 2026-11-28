/*——- Omar Hussein ——-*/
using Microsoft.EntityFrameworkCore;
using MVC.BLL.Common;
using MVC.BLL.DTOs.Accounts;
using MVC.BLL.Interfaces;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Entities.Accounts;
using MVC.DAL.Enums;

namespace MVC.BLL.Services.Accounts
{
    // صفحة Doctor approvals عند الأدمن: المراجعة الأولى + المستندات البديلة
    public class DoctorApprovalService : IDoctorApprovalService
    {
        private const int MaxReasonLength = 500;
        private readonly MediCareDbContext _db;

        public DoctorApprovalService(MediCareDbContext db) => _db = db;

        public async Task<ApprovalCountsDto> GetCountsAsync() => new()
        {
            Initial = await _db.DoctorProfiles.CountAsync(d => d.ApprovalStatus == ApprovalStatus.Pending),
            Replacements = await ReplacementsQuery().CountAsync()
        };

        public async Task<List<ApprovalListItemDto>> GetListAsync(ApprovalTab tab)
        {
            if (tab == ApprovalTab.Replacements)
            {
                return await ReplacementsQuery()
                    .OrderBy(x => x.UploadedAtUtc)
                    .Select(x => new ApprovalListItemDto
                    {
                        DoctorId = x.DoctorId,
                        FullName = x.Doctor.User.FullName,
                        HasPhoto = x.Doctor.User.ProfilePhotoStorageKey != null,
                        Specialty = new SpecialtyDto { SpecialtyId = x.Doctor.SpecialtyId, NameEn = x.Doctor.Specialty.NameEn, NameAr = x.Doctor.Specialty.NameAr },
                        Status = x.ApprovalStatus,
                        SubmittedAtUtc = x.UploadedAtUtc,
                        DocumentId = x.DoctorDocumentId,
                        DocumentType = x.DocumentType
                    })
                    .ToListAsync();
            }

            var status = tab switch
            {
                ApprovalTab.Approved => ApprovalStatus.Approved,
                ApprovalTab.Rejected => ApprovalStatus.Rejected,
                _ => ApprovalStatus.Pending
            };

            var items = await _db.DoctorProfiles
                .Where(d => d.ApprovalStatus == status)
                .Select(d => new ApprovalListItemDto
                {
                    DoctorId = d.DoctorId,
                    FullName = d.User.FullName,
                    HasPhoto = d.User.ProfilePhotoStorageKey != null,
                    Specialty = new SpecialtyDto { SpecialtyId = d.SpecialtyId, NameEn = d.Specialty.NameEn, NameAr = d.Specialty.NameAr },
                    Status = d.ApprovalStatus,
                    // آخر رفع للرخصة أو الشهادة = وقت التقديم
                    SubmittedAtUtc = d.Documents
                        .Max(x => (DateTimeOffset?)x.UploadedAtUtc)
                })
                .ToListAsync();

            // الأقدم الأول في المراجعة، والأحدث الأول في الأرشيف
            return status == ApprovalStatus.Pending
                ? items.OrderBy(i => i.SubmittedAtUtc).ToList()
                : items.OrderByDescending(i => i.SubmittedAtUtc).ToList();
        }

        public async Task<DoctorReviewDto?> GetDoctorReviewAsync(string doctorId)
        {
            var doctor = await _db.DoctorProfiles
                .AsNoTracking()
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Include(d => d.Documents)
                .AsSplitQuery()
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (doctor is null)
                return null;

            var license = LatestForReview(doctor, DoctorDocumentType.License);
            var certificate = LatestForReview(doctor, DoctorDocumentType.GraduationCertificate);

            return new DoctorReviewDto
            {
                DoctorId = doctor.DoctorId,
                FullName = doctor.User.FullName,
                Email = doctor.User.Email ?? string.Empty,
                MaskedPhone = ContactFormat.FormatPhone(doctor.User.PhoneNumber),
                HasPhoto = !string.IsNullOrEmpty(doctor.User.ProfilePhotoStorageKey),
                Specialty = DoctorProfileService.ToDto(doctor.Specialty),
                LicenseNumber = doctor.LicenseNumber,
                University = doctor.University,
                GraduationYear = doctor.GraduationYear,
                YearsOfExperience = Experience.Years(doctor.PracticeStartYear),
                ApprovalStatus = doctor.ApprovalStatus,
                IsActive = doctor.User.IsActive,
                License = license is null ? null : DoctorProfileService.ToDto(license),
                GraduationCertificate = certificate is null ? null : DoctorProfileService.ToDto(certificate),
                RejectionReason = doctor.ApprovalStatus == ApprovalStatus.Rejected
                    ? (license?.RejectionReason ?? certificate?.RejectionReason)
                    : null
            };
        }

        public async Task<ReplacementReviewDto?> GetReplacementReviewAsync(Guid documentId)
        {
            var replacement = await _db.DoctorDocuments
                .AsNoTracking()
                .Include(x => x.ReplacesDocument)
                .Include(x => x.Doctor).ThenInclude(d => d.User)
                .Include(x => x.Doctor).ThenInclude(d => d.Specialty)
                .FirstOrDefaultAsync(x => x.DoctorDocumentId == documentId && x.ReplacesDocumentId != null);
            if (replacement is null)
                return null;

            // لو المستند الأصلي مش هو الحالي (حالة نادرة) بنعرض الحالي الفعلي
            var current = replacement.ReplacesDocument is { IsCurrent: true }
                ? replacement.ReplacesDocument
                : await _db.DoctorDocuments.AsNoTracking().FirstOrDefaultAsync(x =>
                    x.DoctorId == replacement.DoctorId && x.DocumentType == replacement.DocumentType
                    && x.ApprovalStatus == ApprovalStatus.Approved && x.IsCurrent);

            return new ReplacementReviewDto
            {
                DoctorId = replacement.DoctorId,
                FullName = replacement.Doctor.User.FullName,
                HasPhoto = !string.IsNullOrEmpty(replacement.Doctor.User.ProfilePhotoStorageKey),
                Specialty = DoctorProfileService.ToDto(replacement.Doctor.Specialty),
                DoctorStatus = replacement.Doctor.ApprovalStatus,
                Current = current is null ? null : DoctorProfileService.ToDto(current),
                Replacement = DoctorProfileService.ToDto(replacement)
            };
        }

        public async Task<ServiceResult> DecideInitialAsync(string adminId, string doctorId, bool approve, string? rejectionReason)
        {
            var reason = rejectionReason?.Trim();
            if (!approve && string.IsNullOrEmpty(reason))
                return ServiceResult.Fail("A rejection reason is required.");
            if (reason?.Length > MaxReasonLength)
                return ServiceResult.Fail("The reason is too long.");

            var profile = await _db.DoctorProfiles.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (profile is null)
                return ServiceResult.Fail("Doctor profile not found.");
            if (profile.ApprovalStatus != ApprovalStatus.Pending)
                return ServiceResult.Fail("This application has already been reviewed.");

            var pending = await _db.DoctorDocuments
                .Where(x => x.DoctorId == doctorId && x.ApprovalStatus == ApprovalStatus.Pending)
                .ToListAsync();
            var license = pending.FirstOrDefault(x => x.DocumentType == DoctorDocumentType.License);
            var certificate = pending.FirstOrDefault(x => x.DocumentType == DoctorDocumentType.GraduationCertificate);
            if (license is null || certificate is null)
                return ServiceResult.Fail("Both the license and the graduation certificate are needed to decide.");

            var now = DateTimeOffset.UtcNow;
            var status = approve ? ApprovalStatus.Approved : ApprovalStatus.Rejected;

            // قرار واحد: المستندين + حالة الدكتور في transaction واحدة
            await using var transaction = await _db.Database.BeginTransactionAsync();
            foreach (var doc in new[] { license, certificate })
            {
                doc.ApprovalStatus = status;
                doc.IsCurrent = approve;
                doc.ReviewedAtUtc = now;
                doc.ReviewedByAdminId = adminId;
                doc.RejectionReason = approve ? null : reason;
            }
            profile.ApprovalStatus = status;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DecideReplacementAsync(string adminId, Guid documentId, bool approve, string? rejectionReason)
        {
            var reason = rejectionReason?.Trim();
            if (!approve && string.IsNullOrEmpty(reason))
                return ServiceResult.Fail("A rejection reason is required.");
            if (reason?.Length > MaxReasonLength)
                return ServiceResult.Fail("The reason is too long.");

            var replacement = await _db.DoctorDocuments
                .FirstOrDefaultAsync(x => x.DoctorDocumentId == documentId && x.ReplacesDocumentId != null);
            if (replacement is null)
                return ServiceResult.Fail("Document not found.");
            if (replacement.ApprovalStatus != ApprovalStatus.Pending)
                return ServiceResult.Fail("This document has already been reviewed.");

            var now = DateTimeOffset.UtcNow;
            await using var transaction = await _db.Database.BeginTransactionAsync();

            if (approve)
            {
                // خطوة 1: نشيل IsCurrent من القديم الأول عشان الـ filtered unique index (Approved + IsCurrent)
                var current = await _db.DoctorDocuments.Where(x =>
                        x.DoctorId == replacement.DoctorId && x.DocumentType == replacement.DocumentType
                        && x.ApprovalStatus == ApprovalStatus.Approved && x.IsCurrent)
                    .ToListAsync();
                foreach (var old in current)
                    old.IsCurrent = false; // القديم بيفضل Approved للتاريخ
                await _db.SaveChangesAsync();

                // خطوة 2: البديل يبقى هو الحالي
                replacement.ApprovalStatus = ApprovalStatus.Approved;
                replacement.IsCurrent = true;
                replacement.RejectionReason = null;
            }
            else
            {
                // الرفض: القديم بيفضل الحالي زي ما هو
                replacement.ApprovalStatus = ApprovalStatus.Rejected;
                replacement.IsCurrent = false;
                replacement.RejectionReason = reason;
            }
            replacement.ReviewedAtUtc = now;
            replacement.ReviewedByAdminId = adminId;

            // حالة الدكتور مش بتتغير في المستندات البديلة
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return ServiceResult.Success();
        }

        // البدائل المستنية: Pending وبتستبدل مستند، لدكتور معتمد
        private IQueryable<DoctorDocument> ReplacementsQuery() =>
            _db.DoctorDocuments.Where(x =>
                x.ReplacesDocumentId != null
                && x.ApprovalStatus == ApprovalStatus.Pending
                && x.Doctor.ApprovalStatus == ApprovalStatus.Approved);

        // المستند اللي بيتعرض في المراجعة: الـ Pending الأولي، وإلا الحالي، وإلا آخر واحد (غير البدائل)
        private static DoctorDocument? LatestForReview(DoctorProfile doctor, DoctorDocumentType type)
        {
            var docs = doctor.Documents.Where(x => x.DocumentType == type).ToList();
            return docs.FirstOrDefault(x => x.ApprovalStatus == ApprovalStatus.Pending && x.ReplacesDocumentId == null)
                ?? docs.FirstOrDefault(x => x.ApprovalStatus == ApprovalStatus.Approved && x.IsCurrent)
                ?? docs.Where(x => x.ReplacesDocumentId == null).OrderByDescending(x => x.UploadedAtUtc).FirstOrDefault();
        }
    }
}
/*————*/
