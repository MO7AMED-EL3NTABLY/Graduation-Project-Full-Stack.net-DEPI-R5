using MVC.DAL.Entities.Booking;

namespace MVC.DAL.Entities.Locations
{
    // شيفت أسبوعي متكرر (template) - جدول Shifts
    // CHECK: EndTime بعد StartTime في نفس اليوم (مفيش شيفت بيعدي نص الليل)
    public class Shift
    {
        public Guid ShiftId { get; set; }

        public Guid DoctorLocationId { get; set; } // FK لـ DoctorLocations

        public TimeOnly StartTime { get; set; } // وقت البداية (بتوقيت القاهرة)

        public TimeOnly EndTime { get; set; } // وقت النهاية (بتوقيت القاهرة)

        public int MaxPatients { get; set; } // أقصى عدد مرضى في الشيفت

        public bool IsActive { get; set; } // لو اتقفل بيوقف الحجز الجديد

        public bool IsDeleted { get; set; } // حذف منطقي (soft delete)

        // Navigation properties
        public DoctorLocation DoctorLocation { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<BookingHold> BookingHolds { get; set; } = new List<BookingHold>();
        public ICollection<ShiftDay> ShiftDays { get; set; } = new List<ShiftDay>();
        public ICollection<ShiftException> ShiftExceptions { get; set; } = new List<ShiftException>();
    }
}
