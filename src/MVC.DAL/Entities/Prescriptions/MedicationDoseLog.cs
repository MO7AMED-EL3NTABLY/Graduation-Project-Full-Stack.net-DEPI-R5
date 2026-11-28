using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Prescriptions
{
    // Record of a patient's scheduled medicine dose - MedicationDoseLogs table
    // Filtered unique index on (PrescriptionMedicineId, ScheduledAtUtc) WHERE Status <> 'Cancelled'
    // Rows are never deleted, and a dose is never marked Missed automatically
    public class MedicationDoseLog
    {
        public Guid MedicationDoseLogId { get; set; }

        public Guid PrescriptionMedicineId { get; set; } // FK to PrescriptionMedicines

        public DateTimeOffset ScheduledAtUtc { get; set; } // Dose time

        public DoseLogStatus Status { get; set; } // Pending / Taken / Cancelled

        public DateTimeOffset? TakenAtUtc { get; set; } // When the patient tapped Taken

        // Navigation properties
        public PrescriptionMedicine PrescriptionMedicine { get; set; } = null!;
    }
}
