namespace MVC.DAL.Entities.Accounts
{
    // التخصصات الطبية - جدول Specialties
    public class Specialty
    {
        public Guid SpecialtyId { get; set; }

        public string NameAr { get; set; } = null!; // الاسم بالعربي

        public string NameEn { get; set; } = null!; // الاسم بالإنجليزي

        public bool IsActive { get; set; } // لو اتقفل مينفعش يتختار جديد بس بيفضل مع الدكاترة الحاليين

        public string? ImageStorageKey { get; set; } // مفتاح صورة كارت التخصص (اختياري)

        // Navigation properties
        public ICollection<DoctorProfile> Doctors { get; set; } = new List<DoctorProfile>();
    }
}
