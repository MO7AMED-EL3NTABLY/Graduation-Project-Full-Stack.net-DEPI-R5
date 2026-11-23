using MVC.DAL.Entities.Booking;

namespace MVC.DAL.Entities.Locations
{
    // شيفت إضافي لمرة واحدة في تاريخ معين - جدول AdditionalShifts
    // CHECK: EndTime بعد StartTime في نفس اليوم (مفيش شيفت بيعدي نص الليل)
    public class AdditionalShift
    {
        public Guid AdditionalShiftId { get; set; }

        public Guid DoctorLocationId { get; set; } // FK لـ DoctorLocations

        public DateOnly ShiftDate { get; set; } // تاريخ الشيفت (بتوقيت القاهرة)

        public TimeOnly StartTime { get; set; } // وقت البداية

        public TimeOnly EndTime { get; set; } // وقت النهاية

        public int MaxPatients { get; set; } // أقصى عدد مرضى

        public bool IsActive { get; set; } // لو اتقفل بيوقف الحجز الجديد

        public bool IsDeleted { get; set; } // حذف منطقي (soft delete)

        // Navigation properties
        public DoctorLocation DoctorLocation { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<BookingHold> BookingHolds { get; set; } = new List<BookingHold>();
    }
}
