using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Booking
{
    // The patient's rating of the doctor (stars, no text) - Reviews table
    // Filtered unique index: one non-deleted review per (PatientId, DoctorId)
    // Not linked to an appointment, and the average is derived, never stored
    public class Review
    {
        public Guid ReviewId { get; set; }

        public string PatientId { get; set; } = null!; // FK to PatientProfiles

        public string DoctorId { get; set; } = null!; // FK to DoctorProfiles

        public int Rating { get; set; } // Rating from 1 to 5

        public DateTimeOffset CreatedAtUtc { get; set; } // Creation time

        public DateTimeOffset UpdatedAtUtc { get; set; } // Last update time

        public DateTimeOffset? DeletedAtUtc { get; set; } // Soft delete time - null if it still exists

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile Doctor { get; set; } = null!;
    }
}
