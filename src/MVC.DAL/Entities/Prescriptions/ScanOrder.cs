using MVC.DAL.Entities.Notifications;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Prescriptions
{
    // A lab test or scan ordered by the doctor - ScanOrders table
    // Linked to the prescription, not the appointment, and has no status
    public class ScanOrder
    {
        public Guid ScanOrderId { get; set; }

        public Guid PrescriptionId { get; set; } // FK to Prescriptions

        public ScanType ScanType { get; set; } // Lab test / Radiology scan

        public string ScanName { get; set; } = null!; // Lab test or scan name

        public string? Notes { get; set; } // Notes (optional)

        public DateTimeOffset CreatedAtUtc { get; set; } // Creation time

        // Navigation properties
        public Prescription Prescription { get; set; } = null!;
        public ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>(); // Result files
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
