using MVC.DAL.Entities.Notifications;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Prescriptions
{
    // A medicine in the prescription - PrescriptionMedicines table
    // The medicine name is free text (no medicine catalog)
    // CHECK: DurationDays > 0
    public class PrescriptionMedicine
    {
        public Guid PrescriptionMedicineId { get; set; }

        public Guid PrescriptionId { get; set; } // FK to Prescriptions

        public string MedicineName { get; set; } = null!; // Medicine name

        public string Dose { get; set; } = null!; // Dose including the unit

        public int DurationDays { get; set; } // Treatment duration in days - required and greater than zero

        public string Instructions { get; set; } = null!; // Instructions (before/after food...)

        public PrescriptionMedicineStatus Status { get; set; } // Active / Stopped

        public DateTimeOffset? StoppedAtUtc { get; set; } // Medicine stop time

        // Navigation properties
        public Prescription Prescription { get; set; } = null!;
        public ICollection<PrescriptionMedicineTime> Times { get; set; } = new List<PrescriptionMedicineTime>();
        public ICollection<MedicationDoseLog> DoseLogs { get; set; } = new List<MedicationDoseLog>();
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}
