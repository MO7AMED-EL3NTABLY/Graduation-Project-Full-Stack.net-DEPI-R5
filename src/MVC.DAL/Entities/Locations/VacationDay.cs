using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Locations
{
    // إجازة الدكتور (فترة من تاريخ لتاريخ شاملة الطرفين) - جدول VacationDays
    // إجازات نفس الدكتور مينفعش تتداخل، وبتطبق على كل أماكنه
    public class VacationDay
    {
        public Guid VacationDayId { get; set; }

        public string DoctorId { get; set; } = null!; // FK لـ DoctorProfiles

        public DateOnly StartDate { get; set; } // أول يوم إجازة

        public DateOnly EndDate { get; set; } // آخر يوم إجازة

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
    }
}
