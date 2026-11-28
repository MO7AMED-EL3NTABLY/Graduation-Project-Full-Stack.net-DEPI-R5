namespace MVC.DAL.Enums
{
    // Treatment type in the prescription - Prescriptions.TreatmentType
    public enum TreatmentType
    {
        Chronic, // Chronic condition (the prescription is never deleted or hidden)
        NonChronic // Not chronic (can be deleted and is hidden automatically)
    }
}
