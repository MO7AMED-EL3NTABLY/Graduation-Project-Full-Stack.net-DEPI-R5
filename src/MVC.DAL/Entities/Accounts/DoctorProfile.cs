using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Chat;
using MVC.DAL.Entities.Locations;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Accounts
{
    // Doctor profile - DoctorProfiles table
    // DoctorId is both the PK and the FK to AspNetUsers.Id (shared PK)
    public class DoctorProfile
    {
        public string DoctorId { get; set; } = null!; // Same Id as the user

        public Guid SpecialtyId { get; set; } // Specialty (changes only with admin approval)

        public string LicenseNumber { get; set; } = null!; // License number (changes only with admin approval)

        public string University { get; set; } = null!; // University (changes only with admin approval)

        public int GraduationYear { get; set; } // Graduation year (changes only with admin approval)

        public int PracticeStartYear { get; set; } // Practice start year - years of experience are derived from it and never stored (D-03)

        public string Bio { get; set; } = null!; // About the doctor (required)

        public ApprovalStatus ApprovalStatus { get; set; } // Admin approval status of the doctor

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Specialty Specialty { get; set; } = null!;
        public ICollection<DoctorWorkExperience> WorkExperiences { get; set; } = new List<DoctorWorkExperience>();
        public ICollection<DoctorSurgery> Surgeries { get; set; } = new List<DoctorSurgery>();
        public ICollection<DoctorDocument> Documents { get; set; } = new List<DoctorDocument>();
        public ICollection<DoctorProfileChangeRequest> ChangeRequests { get; set; } = new List<DoctorProfileChangeRequest>();
        public ICollection<DoctorLocation> DoctorLocations { get; set; } = new List<DoctorLocation>();
        public ICollection<VacationDay> VacationDays { get; set; } = new List<VacationDay>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<PatientCondition> CreatedConditions { get; set; } = new List<PatientCondition>(); // Chronic conditions recorded by this doctor
        public ICollection<ChatThread> ChatThreads { get; set; } = new List<ChatThread>();
    }
}
