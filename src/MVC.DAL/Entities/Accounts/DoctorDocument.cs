using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Accounts
{
    // مستندات الدكتور (رخصة / شهادة تخرج / غيره) - جدول DoctorDocuments
    public class DoctorDocument
    {
        public Guid DoctorDocumentId { get; set; }

        public string DoctorId { get; set; } = null!; // FK لـ DoctorProfiles

        public DoctorDocumentType DocumentType { get; set; } // نوع المستند

        public string StorageKey { get; set; } = null!; // مفتاح الملف (مش رابط عام)

        public string OriginalFileName { get; set; } = null!; // اسم الملف الأصلي

        public string ContentType { get; set; } = null!; // نوع الملف (pdf, jpg...)

        public long FileSizeBytes { get; set; } // حجم الملف بالبايت

        public ApprovalStatus ApprovalStatus { get; set; } // حالة المراجعة (Other بيفضل Pending)

        public bool IsCurrent { get; set; } // المستند الحالي المعتمد (1 لما يكون Approved بس)

        public string? RejectionReason { get; set; } // سبب الرفض - مطلوب لو Rejected

        public DateTimeOffset? ReviewedAtUtc { get; set; } // وقت المراجعة - null وهو Pending

        public string? ReviewedByAdminId { get; set; } // الأدمن اللي راجع - null وهو Pending

        public Guid? ReplacesDocumentId { get; set; } // المستند القديم اللي ده بيستبدله (self-reference)

        public DateTimeOffset UploadedAtUtc { get; set; } // وقت الرفع

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
        public ApplicationUser? ReviewedByAdmin { get; set; }
        public DoctorDocument? ReplacesDocument { get; set; }
    }
}
