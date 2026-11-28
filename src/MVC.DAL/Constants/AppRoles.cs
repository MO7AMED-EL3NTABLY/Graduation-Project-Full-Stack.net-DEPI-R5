namespace MVC.DAL.Constants
{
    // Role names on the platform - AspNetRoles table
    // Use these constants instead of writing the name as a string anywhere: [Authorize(Roles = AppRoles.Admin)]
    public static class AppRoles
    {
        public const string Patient = "Patient"; // Patient
        public const string Doctor = "Doctor"; // Doctor
        public const string Admin = "Admin"; // Admin

        public static readonly string[] All = { Patient, Doctor, Admin };
    }
}
