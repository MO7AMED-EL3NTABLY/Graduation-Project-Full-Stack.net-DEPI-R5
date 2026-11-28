namespace MVC.DAL.Entities.Accounts
{
    // The doctor's previous work experience - DoctorWorkExperiences table
    public class DoctorWorkExperience
    {
        public Guid DoctorWorkExperienceId { get; set; }

        public string DoctorId { get; set; } = null!; // FK to DoctorProfiles

        public string Organization { get; set; } = null!; // The organization they worked at

        public string Position { get; set; } = null!; // Job title

        public int FromYear { get; set; } // From year

        public int? ToYear { get; set; } // To year - null means still working there

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
    }
}
