namespace MVC.DAL.Enums
{
    // نوع طلب تعديل بيانات الدكتور المحمية اللي محتاج موافقة الأدمن - DoctorProfileChangeRequests.RequestType
    public enum ProfileChangeRequestType
    {
        Specialty, // تغيير التخصص
        LicenseNumber, // تغيير رقم الرخصة
        University, // تغيير الجامعة
        GraduationYear // تغيير سنة التخرج
    }
}
