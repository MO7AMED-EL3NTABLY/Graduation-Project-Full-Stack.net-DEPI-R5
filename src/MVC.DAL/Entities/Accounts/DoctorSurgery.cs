namespace MVC.DAL.Entities.Accounts
{
    // العمليات اللي الدكتور بيعملها - جدول DoctorSurgeries
    public class DoctorSurgery
    {
        public Guid DoctorSurgeryId { get; set; }

        public string DoctorId { get; set; } = null!; // FK لـ DoctorProfiles

        public string SurgeryName { get; set; } = null!; // اسم العملية

        public string? Description { get; set; } // وصف (اختياري)

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
    }
}
