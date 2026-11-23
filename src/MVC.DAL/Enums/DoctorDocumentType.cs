namespace MVC.DAL.Enums
{
    // نوع المستند اللي الدكتور بيرفعه - DoctorDocuments.DocumentType
    public enum DoctorDocumentType
    {
        License, // رخصة مزاولة المهنة (بتتراجع)
        GraduationCertificate, // شهادة التخرج (بتتراجع)
        Other // مستندات تانية اختيارية (مش بتتراجع)
    }
}
