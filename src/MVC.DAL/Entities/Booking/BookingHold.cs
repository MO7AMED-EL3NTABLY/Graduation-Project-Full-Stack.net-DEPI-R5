using MVC.DAL.Entities.Accounts;
using MVC.DAL.Entities.Locations;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Booking
{
    // A temporary two-minute hold on a place until the patient confirms - BookingHolds table
    // CHECK: exactly one of ShiftId and AdditionalShiftId must have a value (XOR)
    // CHECK: NewBooking => OriginalAppointmentId null - Reschedule => OriginalAppointmentId not null
    // The row is physically deleted after confirmation, cancellation or expiry
    public class BookingHold
    {
        public Guid BookingHoldId { get; set; }

        public string PatientId { get; set; } = null!; // FK to PatientProfiles

        public Guid? ShiftId { get; set; } // FK to Shifts (XOR with AdditionalShiftId)

        public Guid? AdditionalShiftId { get; set; } // FK to AdditionalShifts (XOR with ShiftId)

        public DateOnly AppointmentDate { get; set; } // The requested appointment date in Cairo time

        public BookingHoldPurpose Purpose { get; set; } // New booking / reschedule

        public Guid? OriginalAppointmentId { get; set; } // The original appointment - required for Reschedule

        public DateTimeOffset CreatedAtUtc { get; set; } // Creation time on the server

        public DateTimeOffset ExpiresAtUtc { get; set; } // Expiry time (exactly two minutes later)

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public Shift? Shift { get; set; }
        public AdditionalShift? AdditionalShift { get; set; }
        public Appointment? OriginalAppointment { get; set; }
    }
}
