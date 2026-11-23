using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Locations
{
    // استثناء على شيفت في تاريخ معين - جدول ShiftExceptions
    // Unique على (ShiftId, ExceptionDate)
    public class ShiftException
    {
        public Guid ShiftExceptionId { get; set; }

        public Guid ShiftId { get; set; } // FK لـ Shifts

        public DateOnly ExceptionDate { get; set; } // تاريخ الاستثناء (لازم يوم الشيفت شغال فيه)

        public ShiftExceptionType ExceptionType { get; set; } // Blocked / SpecialWorkingHours

        public TimeOnly? StartTime { get; set; } // null لو Blocked - مطلوب لو SpecialWorkingHours

        public TimeOnly? EndTime { get; set; } // null لو Blocked - مطلوب لو SpecialWorkingHours

        public string? Reason { get; set; } // السبب (اختياري)

        // Navigation properties
        public Shift Shift { get; set; } = null!;
    }
}
