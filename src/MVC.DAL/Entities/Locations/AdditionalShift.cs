using MVC.DAL.Entities.Booking;

namespace MVC.DAL.Entities.Locations
{
    // A one-off extra shift on a specific date - AdditionalShifts table
    // CHECK: EndTime after StartTime on the same day (no shift crosses midnight)
    public class AdditionalShift
    {
        public Guid AdditionalShiftId { get; set; }

        public Guid DoctorLocationId { get; set; } // FK to DoctorLocations

        public DateOnly ShiftDate { get; set; } // Shift date (Cairo time)

        public TimeOnly StartTime { get; set; } // Start time

        public TimeOnly EndTime { get; set; } // End time

        public int MaxPatients { get; set; } // Maximum number of patients

        public bool IsActive { get; set; } // When turned off it stops new bookings

        public bool IsDeleted { get; set; } // Soft delete

        // Navigation properties
        public DoctorLocation DoctorLocation { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<BookingHold> BookingHolds { get; set; } = new List<BookingHold>();
    }
}
