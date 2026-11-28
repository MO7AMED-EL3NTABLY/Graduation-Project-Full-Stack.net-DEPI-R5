namespace MVC.DAL.Entities.Accounts
{
    // Surgeries the doctor performs - DoctorSurgeries table
    public class DoctorSurgery
    {
        public Guid DoctorSurgeryId { get; set; }

        public string DoctorId { get; set; } = null!; // FK to DoctorProfiles

        public string SurgeryName { get; set; } = null!; // Surgery name

        public string? Description { get; set; } // Description (optional)

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
    }
}
