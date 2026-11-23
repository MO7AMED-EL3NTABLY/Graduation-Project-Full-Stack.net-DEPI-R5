namespace MVC.DAL.Entities.Accounts
{
    // خبرات الشغل السابقة للدكتور - جدول DoctorWorkExperiences
    public class DoctorWorkExperience
    {
        public Guid DoctorWorkExperienceId { get; set; }

        public string DoctorId { get; set; } = null!; // FK لـ DoctorProfiles

        public string Organization { get; set; } = null!; // المكان اللي اشتغل فيه

        public string Position { get; set; } = null!; // المسمى الوظيفي

        public int FromYear { get; set; } // من سنة

        public int? ToYear { get; set; } // لحد سنة - null يعني لسه شغال فيه

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
    }
}
