namespace MVC.DAL.Enums
{
    // Appointment status - Appointments.Status
    public enum AppointmentStatus
    {
        Confirmed, // Booking confirmed and the visit hasn't happened yet
        Completed, // Visit done (first prescription action)
        Cancelled, // Booking cancelled
        Missed // Patient didn't show up (end of the day, Cairo time)
    }
}
