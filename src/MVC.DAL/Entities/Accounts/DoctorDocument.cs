using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Accounts
{
    // Doctor documents (medical license / graduation certificate) - DoctorDocuments table
    public class DoctorDocument
    {
        public Guid DoctorDocumentId { get; set; }

        public string DoctorId { get; set; } = null!; // FK to DoctorProfiles

        public DoctorDocumentType DocumentType { get; set; } // Document type

        public string StorageKey { get; set; } = null!; // File key (not a public link)

        public string OriginalFileName { get; set; } = null!; // Original file name

        public string ContentType { get; set; } = null!; // File type (pdf, jpg...)

        public long FileSizeBytes { get; set; } // File size in bytes

        public ApprovalStatus ApprovalStatus { get; set; } // Admin review status

        public bool IsCurrent { get; set; } // The current approved document (1 only when Approved)

        public string? RejectionReason { get; set; } // Rejection reason - required when Rejected

        public DateTimeOffset? ReviewedAtUtc { get; set; } // Review time - null while Pending

        public string? ReviewedByAdminId { get; set; } // The reviewing admin - null while Pending

        public Guid? ReplacesDocumentId { get; set; } // The old document this one replaces (self-reference)

        public DateTimeOffset UploadedAtUtc { get; set; } // Upload time

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
        public ApplicationUser? ReviewedByAdmin { get; set; }
        public DoctorDocument? ReplacesDocument { get; set; }
    }
}
