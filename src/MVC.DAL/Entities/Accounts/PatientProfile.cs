using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Chat;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Accounts
{
    // Patient profile - PatientProfiles table
    // PatientId is both the PK and the FK to AspNetUsers.Id (shared PK)
    public class PatientProfile
    {
        public string PatientId { get; set; } = null!; // Same Id as the user

        public DateOnly DateOfBirth { get; set; } // Date of birth (age is derived and never stored)

        public Gender Gender { get; set; } // Male / Female

        public bool ShareMedicalProfile { get; set; } // Share the medical profile with doctors (default false)

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<BookingHold> BookingHolds { get; set; } = new List<BookingHold>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<PatientCondition> Conditions { get; set; } = new List<PatientCondition>();
        public ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>();
        public ICollection<ChatThread> ChatThreads { get; set; } = new List<ChatThread>();
    }
}
