namespace MVC.DAL.Enums
{
    // نوع العلاج في الروشتة - Prescriptions.TreatmentType
    public enum TreatmentType
    {
        Chronic, // مرض مزمن (الروشتة مش بتتمسح ولا بتختفي)
        NonChronic // مش مزمن (ممكن تتمسح وبتختفي تلقائي)
    }
}
