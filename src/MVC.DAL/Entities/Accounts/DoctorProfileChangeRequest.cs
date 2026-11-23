using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Accounts
{
    // طلب تعديل بيانات محمية للدكتور محتاج موافقة الأدمن - جدول DoctorProfileChangeRequests
    // كل طلب بيعدل حقل واحد بس، والعمود اللي يقابل RequestType بس هو اللي يتملى
    public class DoctorProfileChangeRequest
    {
        public Guid DoctorProfileChangeRequestId { get; set; }

        public string DoctorId { get; set; } = null!; // FK لـ DoctorProfiles

        public ProfileChangeRequestType RequestType { get; set; } // الحقل المطلوب تعديله

        public Guid? RequestedSpecialtyId { get; set; } // التخصص الجديد (لو RequestType = Specialty)

        public string? RequestedLicenseNumber { get; set; } // رقم الرخصة الجديد (لو RequestType = LicenseNumber)

        public string? RequestedUniversity { get; set; } // الجامعة الجديدة (لو RequestType = University)

        public int? RequestedGraduationYear { get; set; } // سنة التخرج الجديدة (لو RequestType = GraduationYear)

        public ApprovalStatus Status { get; set; } // حالة الطلب

        public DateTimeOffset? ReviewedAtUtc { get; set; } // وقت المراجعة - null وهو Pending

        public string? ReviewedByAdminId { get; set; } // الأدمن اللي راجع - null وهو Pending

        public string? ReviewReason { get; set; } // سبب القرار (اختياري)

        public DateTimeOffset CreatedAtUtc { get; set; } // وقت إنشاء الطلب

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
        public Specialty? RequestedSpecialty { get; set; }
        public ApplicationUser? ReviewedByAdmin { get; set; }
    }
}
