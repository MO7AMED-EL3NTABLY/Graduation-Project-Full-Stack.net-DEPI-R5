using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Accounts
{
    // A request to change the doctor's protected data that needs admin approval - DoctorProfileChangeRequests table
    // Each request changes one field only, and only the column matching RequestType is filled
    public class DoctorProfileChangeRequest
    {
        public Guid DoctorProfileChangeRequestId { get; set; }

        public string DoctorId { get; set; } = null!; // FK to DoctorProfiles

        public ProfileChangeRequestType RequestType { get; set; } // The field to change

        public Guid? RequestedSpecialtyId { get; set; } // The new specialty (if RequestType = Specialty)

        public string? RequestedLicenseNumber { get; set; } // The new license number (if RequestType = LicenseNumber)

        public string? RequestedUniversity { get; set; } // The new university (if RequestType = University)

        public int? RequestedGraduationYear { get; set; } // The new graduation year (if RequestType = GraduationYear)

        public ApprovalStatus Status { get; set; } // Request status

        public DateTimeOffset? ReviewedAtUtc { get; set; } // Review time - null while Pending

        public string? ReviewedByAdminId { get; set; } // The reviewing admin - null while Pending

        public string? ReviewReason { get; set; } // Reason for the decision (optional)

        public DateTimeOffset CreatedAtUtc { get; set; } // Request creation time

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
        public Specialty? RequestedSpecialty { get; set; }
        public ApplicationUser? ReviewedByAdmin { get; set; }
    }
}
