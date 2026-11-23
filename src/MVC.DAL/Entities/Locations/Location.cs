using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Locations
{
    // مكان الكشف (عيادة أو مستشفى) - جدول Locations
    // كل دكتور ليه مكان خاص بيه حتى لو نفس العنوان، ومفيش إحداثيات في النسخة دي
    public class Location
    {
        public Guid LocationId { get; set; }

        public string Name { get; set; } = null!; // اسم المكان

        public LocationType LocationType { get; set; } // عيادة / مستشفى

        public string Governorate { get; set; } = null!; // المحافظة

        public string City { get; set; } = null!; // المدينة

        public string Address { get; set; } = null!; // العنوان بالتفصيل

        // Navigation properties
        public DoctorLocation? DoctorLocation { get; set; } // المكان ليه DoctorLocation واحد بالكتير
    }
}
