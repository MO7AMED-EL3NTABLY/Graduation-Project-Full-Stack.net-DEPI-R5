namespace MVC.DAL.Entities.Prescriptions
{
    // مواعيد الجرعات اليومية للدوا - جدول PrescriptionMedicineTimes
    // Composite PK (PrescriptionMedicineId, TimeOfDay) ومفيش Id منفصل
    // عدد الجرعات في اليوم = عدد الصفوف
    public class PrescriptionMedicineTime
    {
        public Guid PrescriptionMedicineId { get; set; } // FK لـ PrescriptionMedicines

        public TimeOnly TimeOfDay { get; set; } // ميعاد الجرعة بتوقيت القاهرة

        // Navigation properties
        public PrescriptionMedicine PrescriptionMedicine { get; set; } = null!;
    }
}
