namespace MVC.DAL.Entities.Prescriptions
{
    // Junction table linking chronic prescriptions to chronic conditions (many-to-many) - PrescriptionConditions table
    // Composite PK (PrescriptionId, PatientConditionId) with no other columns
    // The FKs have no cascade delete
    public class PrescriptionCondition
    {
        public Guid PrescriptionId { get; set; } // FK to Prescriptions

        public Guid PatientConditionId { get; set; } // FK to PatientConditions

        // Navigation properties
        public Prescription Prescription { get; set; } = null!;
        public PatientCondition PatientCondition { get; set; } = null!;
    }
}
