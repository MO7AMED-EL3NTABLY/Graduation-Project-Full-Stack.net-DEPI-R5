namespace MVC.DAL.Enums
{
    // نوع المستند اللي الدكتور بيرفعه - DoctorDocuments.DocumentType
    // الدكتور بيرفع المستندين دول بس، والاتنين لازم الأدمن يراجعهم (اتشال Other بقرار التيم)
    public enum DoctorDocumentType
    {
        License, // رخصة مزاولة المهنة (بتتراجع)
        GraduationCertificate // شهادة التخرج (بتتراجع)
    }
}
