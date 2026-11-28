using Microsoft.EntityFrameworkCore;
using MVC.BLL.Common;
using MVC.BLL.DTOs.Accounts;
using MVC.BLL.Interfaces;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Enums;

namespace MVC.BLL.Services.Accounts
{
    // The admin's Profile change requests page
    public class ProfileChangeRequestService : IProfileChangeRequestService
    {
        private const int MaxReasonLength = 500;
        private readonly MediCareDbContext _db;

        public ProfileChangeRequestService(MediCareDbContext db) => _db = db;

        public async Task<ChangeRequestCountsDto> GetCountsAsync()
        {
            var counts = await _db.DoctorProfileChangeRequests
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            int Of(ApprovalStatus s) => counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0;
            return new ChangeRequestCountsDto
            {
                Pending = Of(ApprovalStatus.Pending),
                Approved = Of(ApprovalStatus.Approved),
                Rejected = Of(ApprovalStatus.Rejected)
            };
        }

        public async Task<List<ChangeRequestRowDto>> GetListAsync(ApprovalStatus status)
        {
            var query = _db.DoctorProfileChangeRequests.Where(r => r.Status == status);
            query = status == ApprovalStatus.Pending
                ? query.OrderBy(r => r.CreatedAtUtc)
                : query.OrderByDescending(r => r.ReviewedAtUtc);

            var rows = await query
                .Select(r => new
                {
                    r.DoctorProfileChangeRequestId,
                    r.DoctorId,
                    DoctorName = r.Doctor.User.FullName,
                    r.RequestType,
                    r.Status,
                    r.RequestedLicenseNumber,
                    r.RequestedUniversity,
                    r.RequestedGraduationYear,
                    RequestedSpecialty = r.RequestedSpecialty == null ? null : new SpecialtyDto
                    {
                        SpecialtyId = r.RequestedSpecialty.SpecialtyId,
                        NameEn = r.RequestedSpecialty.NameEn,
                        NameAr = r.RequestedSpecialty.NameAr
                    },
                    CurrentSpecialty = new SpecialtyDto
                    {
                        SpecialtyId = r.Doctor.SpecialtyId,
                        NameEn = r.Doctor.Specialty.NameEn,
                        NameAr = r.Doctor.Specialty.NameAr
                    },
                    r.Doctor.LicenseNumber,
                    r.Doctor.University,
                    r.Doctor.GraduationYear,
                    r.CreatedAtUtc,
                    r.ReviewedAtUtc,
                    r.ReviewReason
                })
                .ToListAsync();

            // The "current" value is what is in the profile now (after approval it equals the requested one)
            return rows.Select(r => new ChangeRequestRowDto
            {
                RequestId = r.DoctorProfileChangeRequestId,
                DoctorId = r.DoctorId,
                DoctorName = r.DoctorName,
                RequestType = r.RequestType,
                Status = r.Status,
                CurrentValue = r.RequestType switch
                {
                    ProfileChangeRequestType.LicenseNumber => r.LicenseNumber,
                    ProfileChangeRequestType.University => r.University,
                    ProfileChangeRequestType.GraduationYear => r.GraduationYear.ToString(),
                    _ => null
                },
                RequestedValue = r.RequestType switch
                {
                    ProfileChangeRequestType.LicenseNumber => r.RequestedLicenseNumber,
                    ProfileChangeRequestType.University => r.RequestedUniversity,
                    ProfileChangeRequestType.GraduationYear => r.RequestedGraduationYear?.ToString(),
                    _ => null
                },
                CurrentSpecialty = r.RequestType == ProfileChangeRequestType.Specialty ? r.CurrentSpecialty : null,
                RequestedSpecialty = r.RequestedSpecialty,
                CreatedAtUtc = r.CreatedAtUtc,
                ReviewedAtUtc = r.ReviewedAtUtc,
                ReviewReason = r.ReviewReason
            }).ToList();
        }

        public async Task<ServiceResult> ApproveAsync(string adminId, Guid requestId)
        {
            var request = await _db.DoctorProfileChangeRequests
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r => r.DoctorProfileChangeRequestId == requestId);
            if (request is null)
                return ServiceResult.Fail("Request not found.");
            if (request.Status != ApprovalStatus.Pending)
                return ServiceResult.Fail("This request has already been reviewed.");

            var profile = request.Doctor;

            // The request + the profile change in one transaction
            await using var transaction = await _db.Database.BeginTransactionAsync();
            switch (request.RequestType)
            {
                case ProfileChangeRequestType.Specialty:
                    var active = await _db.Specialties.AnyAsync(s => s.SpecialtyId == request.RequestedSpecialtyId && s.IsActive);
                    if (!active)
                        return ServiceResult.Fail("The requested specialty is no longer active.");
                    profile.SpecialtyId = request.RequestedSpecialtyId!.Value;
                    break;
                case ProfileChangeRequestType.LicenseNumber:
                    profile.LicenseNumber = request.RequestedLicenseNumber!;
                    break;
                case ProfileChangeRequestType.University:
                    profile.University = request.RequestedUniversity!;
                    break;
                case ProfileChangeRequestType.GraduationYear:
                    profile.GraduationYear = request.RequestedGraduationYear!.Value;
                    break;
            }

            request.Status = ApprovalStatus.Approved;
            request.ReviewedAtUtc = DateTimeOffset.UtcNow;
            request.ReviewedByAdminId = adminId;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RejectAsync(string adminId, Guid requestId, string? reason)
        {
            reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            if (reason?.Length > MaxReasonLength)
                return ServiceResult.Fail("The reason is too long.");

            var request = await _db.DoctorProfileChangeRequests.FirstOrDefaultAsync(r => r.DoctorProfileChangeRequestId == requestId);
            if (request is null)
                return ServiceResult.Fail("Request not found.");
            if (request.Status != ApprovalStatus.Pending)
                return ServiceResult.Fail("This request has already been reviewed.");

            // Rejection doesn't change the profile, and the reason is optional
            request.Status = ApprovalStatus.Rejected;
            request.ReviewedAtUtc = DateTimeOffset.UtcNow;
            request.ReviewedByAdminId = adminId;
            request.ReviewReason = reason;
            await _db.SaveChangesAsync();
            return ServiceResult.Success();
        }
    }
}
