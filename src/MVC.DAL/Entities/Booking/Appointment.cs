using MVC.DAL.Entities.Accounts;
using MVC.DAL.Entities.Locations;
using MVC.DAL.Entities.Notifications;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Booking
{
    // حجز مريض عند دكتور - جدول Appointments
    // CHECK: لازم واحد بس من ShiftId و AdditionalShiftId يكون له قيمة (XOR)
    // Filtered unique index على (PatientId, DoctorId, AppointmentDate) لو Status مش Cancelled
    // الحجوزات مش بتتمسح خالص
    public class Appointment
    {
        public Guid AppointmentId { get; set; }

        public string PatientId { get; set; } = null!; // FK لـ PatientProfiles

        public string DoctorId { get; set; } = null!; // FK لـ DoctorProfiles - متخزن عشان الـ unique index

        public Guid? ShiftId { get; set; } // FK لـ Shifts (XOR مع AdditionalShiftId)

        public Guid? AdditionalShiftId { get; set; } // FK لـ AdditionalShifts (XOR مع ShiftId)

        public DateOnly AppointmentDate { get; set; } // تاريخ الحجز بتوقيت القاهرة

        public decimal VisitFee { get; set; } // سعر الكشف المنسوخ وقت التأكيد - decimal(10,2)

        public AppointmentStatus Status { get; set; } // Confirmed / Completed / Cancelled / Missed

        public DateTimeOffset BookedAtUtc { get; set; } // وقت الحجز

        public DateTimeOffset? CompletedAtUtc { get; set; } // وقت إتمام الزيارة

        public DateTimeOffset? CancelledAtUtc { get; set; } // وقت الإلغاء

        public string? CancelledByUserId { get; set; } // اللي لغى - null لو إلغاء من السيستم

        public string? CancellationReason { get; set; } // سبب الإلغاء (اختياري)

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile Doctor { get; set; } = null!;
        public Shift? Shift { get; set; }
        public AdditionalShift? AdditionalShift { get; set; }
        public ApplicationUser? CancelledByUser { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>(); // الروشتات اللي اتعملت في الحجز ده (الحجز الأصلي)
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
