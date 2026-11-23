using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Notifications
{
    // تذكير بميعاد حجز أو ميعاد دوا - جدول Reminders
    // CHECK: لازم واحد بس من AppointmentId و PrescriptionMedicineId يكون له قيمة (XOR)
    // Filtered unique indexes:
    //   (AppointmentId) WHERE AppointmentId IS NOT NULL AND Status <> 'Cancelled'
    //   (PrescriptionMedicineId, DueAtUtc) WHERE PrescriptionMedicineId IS NOT NULL AND Status <> 'Cancelled'
    public class Reminder
    {
        public Guid ReminderId { get; set; }

        public Guid? AppointmentId { get; set; } // FK لـ Appointments (XOR)

        public Guid? PrescriptionMedicineId { get; set; } // FK لـ PrescriptionMedicines (XOR)

        public DateTimeOffset DueAtUtc { get; set; } // ميعاد إرسال التذكير

        public ReminderStatus Status { get; set; } // Pending / Sent / Cancelled

        // Navigation properties
        public Appointment? Appointment { get; set; }
        public PrescriptionMedicine? PrescriptionMedicine { get; set; }
    }
}
