/*——- Omar Hussein ——-*/
using Microsoft.EntityFrameworkCore;
using MVC.BLL.Common;
using MVC.BLL.Interfaces;
using MVC.DAL.Dbcontexts;
using MVC.DAL.Enums;

namespace MVC.BLL.Services.Accounts
{
    // مين يقدر يشوف أنهي ملف - مفيش ملف بيتفتح من غير المرور على هنا
    public class FileAccessService : IFileAccessService
    {
        private readonly MediCareDbContext _db;
        private readonly IFileStorage _files;

        public FileAccessService(MediCareDbContext db, IFileStorage files)
        {
            _db = db;
            _files = files;
        }

        public async Task<StoredFile?> GetUserPhotoAsync(string? requesterId, bool requesterIsAdmin, string targetUserId)
        {
            var target = await _db.Users
                .Where(u => u.Id == targetUserId)
                .Select(u => new
                {
                    u.ProfilePhotoStorageKey,
                    DoctorStatus = u.DoctorProfile == null ? (ApprovalStatus?)null : u.DoctorProfile.ApprovalStatus
                })
                .FirstOrDefaultAsync();
            if (target is null || string.IsNullOrEmpty(target.ProfilePhotoStorageKey))
                return null;

            // صورة الدكتور المعتمد عامة، وغير كده: صاحبها والأدمن بس
            var allowed = requesterIsAdmin
                || requesterId == targetUserId
                || target.DoctorStatus == ApprovalStatus.Approved;
            if (!allowed)
                return null;

            return Open(target.ProfilePhotoStorageKey, "photo");
        }

        public async Task<StoredFile?> GetDoctorDocumentAsync(string? requesterId, bool requesterIsAdmin, Guid documentId)
        {
            var document = await _db.DoctorDocuments
                .Where(x => x.DoctorDocumentId == documentId)
                .Select(x => new { x.DoctorId, x.StorageKey, x.OriginalFileName })
                .FirstOrDefaultAsync();
            if (document is null)
                return null;

            // المستندات: الدكتور صاحبها والأدمن بس
            if (!requesterIsAdmin && requesterId != document.DoctorId)
                return null;

            return Open(document.StorageKey, document.OriginalFileName);
        }

        private StoredFile? Open(string key, string fileName)
        {
            var stream = _files.OpenRead(key);
            if (stream is null)
                return null;

            var name = fileName == "photo" ? "photo" + Path.GetExtension(key) : fileName;
            return new StoredFile(stream, FileRules.ContentTypeFor(key), name);
        }
    }
}
/*————*/
