using MVC.DAL.Entities.Accounts;
using MVC.DAL.Entities.Locations;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Booking
{
    // حجز مؤقت لمكان لمدة دقيقتين لحد ما المريض يأكد - جدول BookingHolds
    // CHECK: لازم واحد بس من ShiftId و AdditionalShiftId يكون له قيمة (XOR)
    // CHECK: NewBooking => OriginalAppointmentId null - Reschedule => OriginalAppointmentId مش null
    // الصف بيتمسح فعلياً بعد التأكيد أو الإلغاء أو الانتهاء
    public class BookingHold
    {
        public Guid BookingHoldId { get; set; }

        public string PatientId { get; set; } = null!; // FK لـ PatientProfiles

        public Guid? ShiftId { get; set; } // FK لـ Shifts (XOR مع AdditionalShiftId)

        public Guid? AdditionalShiftId { get; set; } // FK لـ AdditionalShifts (XOR مع ShiftId)

        public DateOnly AppointmentDate { get; set; } // تاريخ الحجز المطلوب بتوقيت القاهرة

        public BookingHoldPurpose Purpose { get; set; } // حجز جديد / تغيير ميعاد

        public Guid? OriginalAppointmentId { get; set; } // الحجز الأصلي - مطلوب لو Reschedule

        public DateTimeOffset CreatedAtUtc { get; set; } // وقت الإنشاء على السيرفر

        public DateTimeOffset ExpiresAtUtc { get; set; } // وقت الانتهاء (بعد دقيقتين بالظبط)

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public Shift? Shift { get; set; }
        public AdditionalShift? AdditionalShift { get; set; }
        public Appointment? OriginalAppointment { get; set; }
    }
}
