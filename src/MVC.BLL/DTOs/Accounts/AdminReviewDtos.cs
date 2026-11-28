/*——- Omar Hussein ——-*/
using MVC.DAL.Enums;

namespace MVC.BLL.DTOs.Accounts
{
    // تابات صفحة Doctor approvals
    public enum ApprovalTab
    {
        Initial,      // دكاترة جداد مستنيين المراجعة الأولى
        Replacements, // مستندات بديلة لدكاترة معتمدين
        Approved,
        Rejected
    }

    public class ApprovalCountsDto
    {
        public int Initial { get; set; }
        public int Replacements { get; set; }
    }

    // عنصر في القائمة الشمال
    public class ApprovalListItemDto
    {
        public string DoctorId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public SpecialtyDto Specialty { get; set; } = new();
        public bool HasPhoto { get; set; }
        public ApprovalStatus Status { get; set; }
        public DateTimeOffset? SubmittedAtUtc { get; set; }

        // لتاب Replacements: المستند البديل
        public Guid? DocumentId { get; set; }
        public DoctorDocumentType? DocumentType { get; set; }
    }

    // تفاصيل الدكتور في المراجعة الأولى (أو في تابات Approved / Rejected)
    public class DoctorReviewDto
    {
        public string DoctorId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MaskedPhone { get; set; } = string.Empty;
        public bool HasPhoto { get; set; }
        public SpecialtyDto Specialty { get; set; } = new();
        public string LicenseNumber { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public int GraduationYear { get; set; }
        public int YearsOfExperience { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public bool IsActive { get; set; }

        // آخر رخصة وآخر شهادة تخرج (الـ Pending في المراجعة الأولى)
        public DoctorDocumentDto? License { get; set; }
        public DoctorDocumentDto? GraduationCertificate { get; set; }
        public string? RejectionReason { get; set; }
    }

    // مقارنة المستند الحالي بالبديل
    public class ReplacementReviewDto
    {
        public string DoctorId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool HasPhoto { get; set; }
        public SpecialtyDto Specialty { get; set; } = new();
        public ApprovalStatus DoctorStatus { get; set; }
        public DoctorDocumentDto? Current { get; set; }
        public DoctorDocumentDto Replacement { get; set; } = new();
    }

    // صف في جدول Profile change requests
    public class ChangeRequestRowDto
    {
        public Guid RequestId { get; set; }
        public string DoctorId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public ProfileChangeRequestType RequestType { get; set; }
        public ApprovalStatus Status { get; set; }

        // القيمة الحالية والمطلوبة (للتخصص بنبعت الاسمين عربي وإنجليزي)
        public string? CurrentValue { get; set; }
        public string? RequestedValue { get; set; }
        public SpecialtyDto? CurrentSpecialty { get; set; }
        public SpecialtyDto? RequestedSpecialty { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset? ReviewedAtUtc { get; set; }
        public string? ReviewReason { get; set; }
    }

    public class ChangeRequestCountsDto
    {
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }
}
/*————*/
