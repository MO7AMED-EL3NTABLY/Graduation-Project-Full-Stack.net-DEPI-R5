using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Notifications
{
    // Reminder for an appointment or a medicine time - Reminders table
    // CHECK: exactly one of AppointmentId and PrescriptionMedicineId must have a value (XOR)
    // Filtered unique indexes:
    //   (AppointmentId) WHERE AppointmentId IS NOT NULL AND Status <> 'Cancelled'
    //   (PrescriptionMedicineId, DueAtUtc) WHERE PrescriptionMedicineId IS NOT NULL AND Status <> 'Cancelled'
    public class Reminder
    {
        public Guid ReminderId { get; set; }

        public Guid? AppointmentId { get; set; } // FK to Appointments (XOR)

        public Guid? PrescriptionMedicineId { get; set; } // FK to PrescriptionMedicines (XOR)

        public DateTimeOffset DueAtUtc { get; set; } // When the reminder is sent

        public ReminderStatus Status { get; set; } // Pending / Sent / Cancelled

        // Navigation properties
        public Appointment? Appointment { get; set; }
        public PrescriptionMedicine? PrescriptionMedicine { get; set; }
    }
}
