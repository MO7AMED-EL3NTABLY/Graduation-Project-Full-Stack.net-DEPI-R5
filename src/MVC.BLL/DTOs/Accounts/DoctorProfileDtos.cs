using MVC.DAL.Enums;

namespace MVC.BLL.DTOs.Accounts
{
    // The doctor's Professional profile page
    public class DoctorProfileDto
    {
        public string DoctorId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool HasPhoto { get; set; }
        public int PracticeStartYear { get; set; }
        public int YearsOfExperience { get; set; } // Derived from PracticeStartYear
        public string Bio { get; set; } = string.Empty;
        public ApprovalStatus ApprovalStatus { get; set; }

        // Protected data (the currently approved value)
        public SpecialtyDto Specialty { get; set; } = new();
        public string LicenseNumber { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public int GraduationYear { get; set; }

        // Requests still Pending for each field
        public List<PendingChangeDto> PendingChanges { get; set; } = new();

        public List<DoctorDocumentDto> Documents { get; set; } = new();
        public List<DoctorSurgeryDto> Surgeries { get; set; } = new();
        public List<DoctorWorkExperienceDto> WorkExperiences { get; set; } = new();

        // Specialties available for a specialty change request
        public List<SpecialtyDto> ActiveSpecialties { get; set; } = new();
    }

    public class PendingChangeDto
    {
        public ProfileChangeRequestType RequestType { get; set; }
        public string? RequestedValue { get; set; }        // For text and numeric fields
        public SpecialtyDto? RequestedSpecialty { get; set; } // If the request is a specialty change
    }

    public class DoctorDocumentDto
    {
        public Guid DoctorDocumentId { get; set; }
        public DoctorDocumentType DocumentType { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsReplacement { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public DateTimeOffset UploadedAtUtc { get; set; }
        public DateTimeOffset? ReviewedAtUtc { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class DoctorSurgeryDto
    {
        public Guid DoctorSurgeryId { get; set; }
        public string SurgeryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class DoctorWorkExperienceDto
    {
        public Guid DoctorWorkExperienceId { get; set; }
        public string Organization { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int FromYear { get; set; }
        public int? ToYear { get; set; }
    }
}