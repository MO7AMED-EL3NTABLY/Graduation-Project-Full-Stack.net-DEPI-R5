using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Locations
{
    // Links the doctor to a practice location and its visit fee - DoctorLocations table
    // Unique index on LocationId: one location for one DoctorLocation
    public class DoctorLocation
    {
        public Guid DoctorLocationId { get; set; }

        public string DoctorId { get; set; } = null!; // FK to DoctorProfiles

        public Guid LocationId { get; set; } // FK to Locations (unique)

        public decimal VisitFee { get; set; } // Visit fee in EGP - decimal(10,2) - copied into the appointment

        public bool IsActive { get; set; } // When turned off it cancels the confirmed appointments that haven't happened yet

        public bool IsDeleted { get; set; } // Soft delete

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
        public Location Location { get; set; } = null!;
        public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
        public ICollection<AdditionalShift> AdditionalShifts { get; set; } = new List<AdditionalShift>();
    }
}
