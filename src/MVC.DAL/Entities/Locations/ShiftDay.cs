namespace MVC.DAL.Entities.Locations
{
    // أيام الأسبوع اللي الشيفت شغال فيها - جدول ShiftDays
    // Composite PK (ShiftId, DayOfWeek) ومفيش Id منفصل
    public class ShiftDay
    {
        public Guid ShiftId { get; set; } // FK لـ Shifts

        public DayOfWeek DayOfWeek { get; set; } // يوم الأسبوع (System.DayOfWeek)

        // Navigation properties
        public Shift Shift { get; set; } = null!;
    }
}
