namespace MVC.DAL.Constants
{
    // أسماء الـ Roles في المنصة - جدول AspNetRoles
    // استخدم الثوابت دي بدل ما تكتب الاسم كـ string في أي حتة: [Authorize(Roles = AppRoles.Admin)]
    public static class AppRoles
    {
        public const string Patient = "Patient"; // مريض
        public const string Doctor = "Doctor"; // دكتور
        public const string Admin = "Admin"; // أدمن

        public static readonly string[] All = { Patient, Doctor, Admin };
    }
}
