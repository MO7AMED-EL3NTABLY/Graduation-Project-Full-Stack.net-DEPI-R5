namespace MVC.DAL.Enums
{
    // Medicine status in the prescription - PrescriptionMedicines.Status
    public enum PrescriptionMedicineStatus
    {
        Active, // Medicine active
        Stopped // Medicine stopped (the doctor stopped it or its duration ended)
    }
}
