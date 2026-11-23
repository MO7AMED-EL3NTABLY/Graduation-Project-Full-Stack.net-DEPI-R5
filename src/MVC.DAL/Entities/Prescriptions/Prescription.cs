using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Notifications;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Prescriptions
{
    // الروشتة - جدول Prescriptions
    // AppointmentId هو الحجز الأصلي اللي اتعملت فيه ومبيتغيرش مع Renew أو Finish
    // الدكتور اللي عملها هو بس اللي يعدل/يجدد/ينهي/يمسح (NonChronic بس اللي ينفع يتمسح)
    public class Prescription
    {
        public Guid PrescriptionId { get; set; }

        public Guid AppointmentId { get; set; } // FK لـ Appointments (الحجز الأصلي)

        public string Diagnosis { get; set; } = null!; // التشخيص (نص حر)

        public string Complaint { get; set; } = null!; // شكوى المريض

        public string? TreatmentNotes { get; set; } // ملاحظات العلاج (اختياري)

        public TreatmentType TreatmentType { get; set; } // مزمن / مش مزمن - الدكتور بس اللي يحدده

        public PrescriptionStatus Status { get; set; } // Active / Finished

        public DateTimeOffset? FinishedAtUtc { get; set; } // وقت انتهاء الروشتة

        public DateTimeOffset? VisibleUntilUtc { get; set; } // بعد الوقت ده الروشتة بتختفي (NonChronic بس)

        // Navigation properties
        public Appointment Appointment { get; set; } = null!;
        public ICollection<PrescriptionMedicine> Medicines { get; set; } = new List<PrescriptionMedicine>();
        public ICollection<ScanOrder> ScanOrders { get; set; } = new List<ScanOrder>();
        public ICollection<PrescriptionCondition> PrescriptionConditions { get; set; } = new List<PrescriptionCondition>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
