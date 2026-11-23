using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Prescriptions
{
    // مرض مزمن مؤكد من دكتور للمريض - جدول PatientConditions
    // الاسم نص حر (مفيش كتالوج أمراض)
    // الدكتور اللي عمله (CreatedByDoctorId) هو بس اللي يعدله أو يمسحه
    public class PatientCondition
    {
        public Guid PatientConditionId { get; set; }

        public string PatientId { get; set; } = null!; // FK لـ PatientProfiles

        public string CreatedByDoctorId { get; set; } = null!; // FK لـ DoctorProfiles - الدكتور المسؤول عنه

        public string Name { get; set; } = null!; // اسم المرض

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public DoctorProfile CreatedByDoctor { get; set; } = null!;
        public ICollection<PrescriptionCondition> PrescriptionConditions { get; set; } = new List<PrescriptionCondition>();
    }
}
