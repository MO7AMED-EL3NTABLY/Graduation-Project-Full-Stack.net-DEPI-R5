using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Locations
{
    // ربط الدكتور بمكان كشف وسعر الكشف فيه - جدول DoctorLocations
    // LocationId عليه unique index: مكان واحد لـ DoctorLocation واحد
    public class DoctorLocation
    {
        public Guid DoctorLocationId { get; set; }

        public string DoctorId { get; set; } = null!; // FK لـ DoctorProfiles

        public Guid LocationId { get; set; } // FK لـ Locations (unique)

        public decimal VisitFee { get; set; } // سعر الكشف بالجنيه - decimal(10,2) - بيتنسخ في الحجز

        public bool IsActive { get; set; } // لو اتقفل بيلغي الحجوزات المتأكدة اللي لسه متمتش

        public bool IsDeleted { get; set; } // حذف منطقي (soft delete)

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
        public Location Location { get; set; } = null!;
        public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
        public ICollection<AdditionalShift> AdditionalShifts { get; set; } = new List<AdditionalShift>();
    }
}
