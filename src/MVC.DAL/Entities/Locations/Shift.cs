using MVC.DAL.Entities.Booking;

namespace MVC.DAL.Entities.Locations
{
    // A recurring weekly shift (template) - Shifts table
    // CHECK: EndTime after StartTime on the same day (no shift crosses midnight)
    public class Shift
    {
        public Guid ShiftId { get; set; }

        public Guid DoctorLocationId { get; set; } // FK to DoctorLocations

        public TimeOnly StartTime { get; set; } // Start time (Cairo time)

        public TimeOnly EndTime { get; set; } // End time (Cairo time)

        public int MaxPatients { get; set; } // Maximum number of patients in the shift

        public bool IsActive { get; set; } // When turned off it stops new bookings

        public bool IsDeleted { get; set; } // Soft delete

        // Navigation properties
        public DoctorLocation DoctorLocation { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<BookingHold> BookingHolds { get; set; } = new List<BookingHold>();
        public ICollection<ShiftDay> ShiftDays { get; set; } = new List<ShiftDay>();
        public ICollection<ShiftException> ShiftExceptions { get; set; } = new List<ShiftException>();
    }
}
