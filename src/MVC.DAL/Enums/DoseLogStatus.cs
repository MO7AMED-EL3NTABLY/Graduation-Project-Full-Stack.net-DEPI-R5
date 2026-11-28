namespace MVC.DAL.Enums
{
    // Status of a patient's scheduled medicine dose - MedicationDoseLogs.Status
    public enum DoseLogStatus
    {
        Pending, // Dose not taken yet
        Taken, // The patient tapped that they took it
        Cancelled // Dose cancelled (stop, renew or Finish)
    }
}
