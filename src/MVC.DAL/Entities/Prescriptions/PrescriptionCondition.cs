namespace MVC.DAL.Entities.Prescriptions
{
    // جدول وسيط بيربط الروشتات المزمنة بالأمراض المزمنة (many-to-many) - جدول PrescriptionConditions
    // Composite PK (PrescriptionId, PatientConditionId) ومفيش أعمدة تانية
    // الـ FKs من غير cascade delete
    public class PrescriptionCondition
    {
        public Guid PrescriptionId { get; set; } // FK لـ Prescriptions

        public Guid PatientConditionId { get; set; } // FK لـ PatientConditions

        // Navigation properties
        public Prescription Prescription { get; set; } = null!;
        public PatientCondition PatientCondition { get; set; } = null!;
    }
}
