using MVC.BLL.DTOs.Accounts;
using MVC.DAL.Enums;

namespace MVC.PL.ViewModels.Accounts
{
    // Localization keys for enums and names repeated across the views
    public static class AccountsText
    {
        public static string DocumentType(DoctorDocumentType type) => type switch
        {
            DoctorDocumentType.GraduationCertificate => "Graduation certificate",
            _ => "Medical license"
        };

        public static string ChangeField(ProfileChangeRequestType type) => type switch
        {
            ProfileChangeRequestType.Specialty => "Specialty",
            ProfileChangeRequestType.LicenseNumber => "License number",
            ProfileChangeRequestType.University => "University",
            _ => "Graduation year"
        };

        public static string Status(ApprovalStatus status) => status switch
        {
            ApprovalStatus.Approved => "Approved",
            ApprovalStatus.Rejected => "Rejected",
            _ => "Pending"
        };

        public static string BadgeClass(ApprovalStatus status) => status switch
        {
            ApprovalStatus.Approved => "mc-badge--success",
            ApprovalStatus.Rejected => "mc-badge--danger",
            _ => "mc-badge--warning"
        };

        public static string Initials(string? name) =>
            string.Concat((name ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => p[0])).ToUpperInvariant();

        public static string SpecialtyName(SpecialtyDto? s, bool arabic) => s is null ? string.Empty : arabic ? s.NameAr : s.NameEn;

        // The date in Egypt time (values are stored in UTC)
        public static DateTimeOffset Local(DateTimeOffset utc)
        {
            try
            {
                var zone = TimeZoneInfo.FindSystemTimeZoneById(OperatingSystem.IsWindows() ? "Egypt Standard Time" : "Africa/Cairo");
                return TimeZoneInfo.ConvertTime(utc, zone);
            }
            catch (TimeZoneNotFoundException)
            {
                return utc;
            }
        }
    }
}
