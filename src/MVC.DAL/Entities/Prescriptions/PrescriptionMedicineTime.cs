namespace MVC.DAL.Entities.Prescriptions
{
    // The medicine's daily dose times - PrescriptionMedicineTimes table
    // Composite PK (PrescriptionMedicineId, TimeOfDay) with no separate Id
    // Doses per day = number of rows
    public class PrescriptionMedicineTime
    {
        public Guid PrescriptionMedicineId { get; set; } // FK to PrescriptionMedicines

        public TimeOnly TimeOfDay { get; set; } // Dose time in Cairo time

        // Navigation properties
        public PrescriptionMedicine PrescriptionMedicine { get; set; } = null!;
    }
}
