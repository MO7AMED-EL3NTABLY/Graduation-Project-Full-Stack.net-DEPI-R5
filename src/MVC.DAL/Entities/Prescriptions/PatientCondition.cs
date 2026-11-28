using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Prescriptions
{
    // A doctor-confirmed chronic condition of the patient - PatientConditions table
    // The name is free text (no conditions catalog)
    // Only the doctor who created it (CreatedByDoctorId) can edit or delete it
    public class PatientCondition
    {
        public Guid PatientConditionId { get; set; }

        public string PatientId { get; set; } = null!; // FK to PatientProfiles

        public string CreatedByDoctorId { get; set; } = null!; // FK to DoctorProfiles - the doctor responsible for it

        public string Name { get; set; } = null!; // Condition name

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile CreatedByDoctor { get; set; } = null!;
        public ICollection<PrescriptionCondition> PrescriptionConditions { get; set; } = new List<PrescriptionCondition>();
    }
}
