namespace MVC.DAL.Entities.Locations
{
    // Weekdays on which the shift runs - ShiftDays table
    // Composite PK (ShiftId, DayOfWeek) with no separate Id
    public class ShiftDay
    {
        public Guid ShiftId { get; set; } // FK to Shifts

        public DayOfWeek DayOfWeek { get; set; } // Day of the week (System.DayOfWeek)

        // Navigation properties
        public Shift Shift { get; set; } = null!;
    }
}
