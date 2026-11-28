using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Notifications;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Prescriptions
{
    // Prescription - Prescriptions table
    // AppointmentId is the original appointment it was created in and doesn't change on Renew or Finish
    // Only the doctor who created it can edit/renew/finish/delete it (only NonChronic can be deleted)
    public class Prescription
    {
        public Guid PrescriptionId { get; set; }

        public Guid AppointmentId { get; set; } // FK to Appointments (the original appointment)

        public string Diagnosis { get; set; } = null!; // Diagnosis (free text)

        public string Complaint { get; set; } = null!; // Patient complaint

        public string? TreatmentNotes { get; set; } // Treatment notes (optional)

        public TreatmentType TreatmentType { get; set; } // Chronic / not chronic - set by the doctor only

        public PrescriptionStatus Status { get; set; } // Active / Finished

        public DateTimeOffset? FinishedAtUtc { get; set; } // Prescription finish time

        public DateTimeOffset? VisibleUntilUtc { get; set; } // After this time the prescription is hidden (NonChronic only)

        // Navigation properties
        public Appointment Appointment { get; set; } = null!;
        public ICollection<PrescriptionMedicine> Medicines { get; set; } = new List<PrescriptionMedicine>();
        public ICollection<ScanOrder> ScanOrders { get; set; } = new List<ScanOrder>();
        public ICollection<PrescriptionCondition> PrescriptionConditions { get; set; } = new List<PrescriptionCondition>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
