namespace MVC.DAL.Enums
{
    // حالة المراجعة من الأدمن - بيستخدم في DoctorProfiles و DoctorDocuments و DoctorProfileChangeRequests
    public enum ApprovalStatus
    {
        Pending, // مستني مراجعة الأدمن
        Approved, // الأدمن وافق
        Rejected // الأدمن رفض
    }
}
