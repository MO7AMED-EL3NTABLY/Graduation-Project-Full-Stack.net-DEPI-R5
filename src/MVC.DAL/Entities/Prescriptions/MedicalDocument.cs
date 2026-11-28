using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Prescriptions
{
    // A patient's medical file (lab/scan result or old report) - MedicalDocuments table
    // Each row = one file, and no extracted text or AI output is stored
    public class MedicalDocument
    {
        public Guid MedicalDocumentId { get; set; }

        public string PatientId { get; set; } = null!; // FK to PatientProfiles

        public Guid? ScanOrderId { get; set; } // FK to ScanOrders - null for old reports without an order

        public string StorageKey { get; set; } = null!; // File key (not a public link)

        public string OriginalFileName { get; set; } = null!; // Original file name

        public string ContentType { get; set; } = null!; // File type

        public long FileSizeBytes { get; set; } // File size in bytes

        public DateTimeOffset UploadedAtUtc { get; set; } // Upload time

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public ScanOrder? ScanOrder { get; set; }
    }
}
