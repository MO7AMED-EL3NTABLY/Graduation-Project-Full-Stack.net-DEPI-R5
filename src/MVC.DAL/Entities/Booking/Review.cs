using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Booking
{
    // تقييم المريض للدكتور (نجوم من غير نص) - جدول Reviews
    // Filtered unique index: تقييم واحد مش ممسوح لكل (PatientId, DoctorId)
    // مش مربوط بحجز، والمتوسط بيتحسب ومش بيتخزن
    public class Review
    {
        public Guid ReviewId { get; set; }

        public string PatientId { get; set; } = null!; // FK لـ PatientProfiles

        public string DoctorId { get; set; } = null!; // FK لـ DoctorProfiles

        public int Rating { get; set; } // التقييم من 1 لـ 5

        public DateTimeOffset CreatedAtUtc { get; set; } // وقت الإنشاء

        public DateTimeOffset UpdatedAtUtc { get; set; } // وقت آخر تعديل

        public DateTimeOffset? DeletedAtUtc { get; set; } // وقت الحذف المنطقي - null لو موجود

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile Doctor { get; set; } = null!;
    }
}
