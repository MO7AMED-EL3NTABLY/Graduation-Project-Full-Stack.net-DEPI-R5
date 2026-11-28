using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Locations
{
    // Doctor vacation (a date range including both ends) - VacationDays table
    // Vacations of the same doctor can't overlap, and they apply to all of the doctor's locations
    public class VacationDay
    {
        public Guid VacationDayId { get; set; }

        public string DoctorId { get; set; } = null!; // FK to DoctorProfiles

        public DateOnly StartDate { get; set; } // First vacation day

        public DateOnly EndDate { get; set; } // Last vacation day

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
    }
}
