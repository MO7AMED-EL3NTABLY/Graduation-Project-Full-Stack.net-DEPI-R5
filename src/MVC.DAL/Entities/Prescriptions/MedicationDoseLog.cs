using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Prescriptions
{
    // سجل جرعة دوا مجدولة للمريض - جدول MedicationDoseLogs
    // Filtered unique index على (PrescriptionMedicineId, ScheduledAtUtc) WHERE Status <> 'Cancelled'
    // الصفوف مش بتتمسح، والجرعة مش بتتعلم Missed لوحدها
    public class MedicationDoseLog
    {
        public Guid MedicationDoseLogId { get; set; }

        public Guid PrescriptionMedicineId { get; set; } // FK لـ PrescriptionMedicines

        public DateTimeOffset ScheduledAtUtc { get; set; } // ميعاد الجرعة

        public DoseLogStatus Status { get; set; } // Pending / Taken / Cancelled

        public DateTimeOffset? TakenAtUtc { get; set; } // وقت ما المريض داس Taken

        // Navigation properties
        public PrescriptionMedicine PrescriptionMedicine { get; set; } = null!;
    }
}
