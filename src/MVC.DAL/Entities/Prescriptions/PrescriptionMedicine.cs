using MVC.DAL.Entities.Notifications;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Prescriptions
{
    // دوا في الروشتة - جدول PrescriptionMedicines
    // اسم الدوا نص حر (مفيش كتالوج أدوية)
    // CHECK: DurationDays > 0
    public class PrescriptionMedicine
    {
        public Guid PrescriptionMedicineId { get; set; }

        public Guid PrescriptionId { get; set; } // FK لـ Prescriptions

        public string MedicineName { get; set; } = null!; // اسم الدوا

        public string Dose { get; set; } = null!; // الجرعة ومعاها الوحدة

        public int DurationDays { get; set; } // مدة العلاج بالأيام - مطلوبة وأكبر من صفر

        public string Instructions { get; set; } = null!; // التعليمات (قبل/بعد الأكل...)

        public PrescriptionMedicineStatus Status { get; set; } // Active / Stopped

        public DateTimeOffset? StoppedAtUtc { get; set; } // وقت إيقاف الدوا

        // Navigation properties
        public Prescription Prescription { get; set; } = null!;
        public ICollection<PrescriptionMedicineTime> Times { get; set; } = new List<PrescriptionMedicineTime>();
        public ICollection<MedicationDoseLog> DoseLogs { get; set; } = new List<MedicationDoseLog>();
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}
