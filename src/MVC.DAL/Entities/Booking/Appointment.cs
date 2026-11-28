using MVC.DAL.Entities.Accounts;
using MVC.DAL.Entities.Locations;
using MVC.DAL.Entities.Notifications;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Booking
{
    // A patient's appointment with a doctor - Appointments table
    // CHECK: exactly one of ShiftId and AdditionalShiftId must have a value (XOR)
    // Filtered unique index on (PatientId, DoctorId, AppointmentDate) when Status is not Cancelled
    // Appointments are never deleted
    public class Appointment
    {
        public Guid AppointmentId { get; set; }

        public string PatientId { get; set; } = null!; // FK to PatientProfiles

        public string DoctorId { get; set; } = null!; // FK to DoctorProfiles - stored for the unique index

        public Guid? ShiftId { get; set; } // FK to Shifts (XOR with AdditionalShiftId)

        public Guid? AdditionalShiftId { get; set; } // FK to AdditionalShifts (XOR with ShiftId)

        public DateOnly AppointmentDate { get; set; } // Appointment date in Cairo time

        public decimal VisitFee { get; set; } // Visit fee copied at confirmation - decimal(10,2)

        public AppointmentStatus Status { get; set; } // Confirmed / Completed / Cancelled / Missed

        public DateTimeOffset BookedAtUtc { get; set; } // Booking time

        public DateTimeOffset? CompletedAtUtc { get; set; } // Visit completion time

        public DateTimeOffset? CancelledAtUtc { get; set; } // Cancellation time

        public string? CancelledByUserId { get; set; } // Who cancelled - null for a system cancellation

        public string? CancellationReason { get; set; } // Cancellation reason (optional)

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile Doctor { get; set; } = null!;
        public Shift? Shift { get; set; }
        public AdditionalShift? AdditionalShift { get; set; }
        public ApplicationUser? CancelledByUser { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>(); // Prescriptions created in this appointment (the original appointment)
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
