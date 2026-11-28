using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Locations
{
    // An exception on a shift on a specific date - ShiftExceptions table
    // Unique on (ShiftId, ExceptionDate)
    public class ShiftException
    {
        public Guid ShiftExceptionId { get; set; }

        public Guid ShiftId { get; set; } // FK to Shifts

        public DateOnly ExceptionDate { get; set; } // Exception date (must be a day the shift runs)

        public ShiftExceptionType ExceptionType { get; set; } // Blocked / SpecialWorkingHours

        public TimeOnly? StartTime { get; set; } // null when Blocked - required when SpecialWorkingHours

        public TimeOnly? EndTime { get; set; } // null when Blocked - required when SpecialWorkingHours

        public string? Reason { get; set; } // Reason (optional)

        // Navigation properties
        public Shift Shift { get; set; } = null!;
    }
}
