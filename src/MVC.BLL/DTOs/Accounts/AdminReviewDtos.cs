using MVC.DAL.Enums;

namespace MVC.BLL.DTOs.Accounts
{
    // Doctor approvals page tabs
    public enum ApprovalTab
    {
        Initial,      // New doctors waiting for the initial review
        Replacements, // Replacement documents for approved doctors
        Approved,
        Rejected
    }

    public class ApprovalCountsDto
    {
        public int Initial { get; set; }
        public int Replacements { get; set; }
    }

    // An item in the left list
    public class ApprovalListItemDto
    {
        public string DoctorId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public SpecialtyDto Specialty { get; set; } = new();
        public bool HasPhoto { get; set; }
        public ApprovalStatus Status { get; set; }
        public DateTimeOffset? SubmittedAtUtc { get; set; }

        // For the Replacements tab: the replacement document
        public Guid? DocumentId { get; set; }
        public DoctorDocumentType? DocumentType { get; set; }
    }

    // Doctor details in the initial review (or in the Approved / Rejected tabs)
    public class DoctorReviewDto
    {
        public string DoctorId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MaskedPhone { get; set; } = string.Empty;
        public bool HasPhoto { get; set; }
        public SpecialtyDto Specialty { get; set; } = new();
        public string LicenseNumber { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public int GraduationYear { get; set; }
        public int YearsOfExperience { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public bool IsActive { get; set; }

        // Latest license and latest graduation certificate (the Pending ones in the initial review)
        public DoctorDocumentDto? License { get; set; }
        public DoctorDocumentDto? GraduationCertificate { get; set; }
        public string? RejectionReason { get; set; }
    }

    // Compares the current document with the replacement
    public class ReplacementReviewDto
    {
        public string DoctorId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool HasPhoto { get; set; }
        public SpecialtyDto Specialty { get; set; } = new();
        public ApprovalStatus DoctorStatus { get; set; }
        public DoctorDocumentDto? Current { get; set; }
        public DoctorDocumentDto Replacement { get; set; } = new();
    }

    // A row in the Profile change requests table
    public class ChangeRequestRowDto
    {
        public Guid RequestId { get; set; }
        public string DoctorId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public ProfileChangeRequestType RequestType { get; set; }
        public ApprovalStatus Status { get; set; }

        // Current and requested values (for specialty we send both Arabic and English names)
        public string? CurrentValue { get; set; }
        public string? RequestedValue { get; set; }
        public SpecialtyDto? CurrentSpecialty { get; set; }
        public SpecialtyDto? RequestedSpecialty { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset? ReviewedAtUtc { get; set; }
        public string? ReviewReason { get; set; }
    }

    public class ChangeRequestCountsDto
    {
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }
}