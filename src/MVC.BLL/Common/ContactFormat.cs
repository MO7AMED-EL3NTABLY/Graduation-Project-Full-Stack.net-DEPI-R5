using System.Text.RegularExpressions;

namespace MVC.BLL.Common
{
    // Normalizes Egyptian mobile numbers and masks part of the email and number on screens
    public static class ContactFormat
    {
        // Accepts: 01012345678 / +201012345678 / 00201012345678 / 201012345678
        private static readonly Regex EgyptMobile = new(@"^(?:\+20|0020|20|0)?(1[0125]\d{8})$", RegexOptions.Compiled);

        public static bool TryNormalizePhone(string? input, out string normalized)
        {
            normalized = string.Empty;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var cleaned = Regex.Replace(input, @"[\s\-()]", "");
            var match = EgyptMobile.Match(cleaned);
            if (!match.Success)
                return false;

            normalized = "+20" + match.Groups[1].Value; // Always stored as +20XXXXXXXXXX
            return true;
        }

        // +201001234567 -> +20 100 •••• 567
        public static string MaskPhone(string? phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 13)
                return phone ?? string.Empty;
            return $"+20 {phone.Substring(3, 3)} •••• {phone[^3..]}";
        }

        // +201001234567 -> +20 100 123 4567
        public static string FormatPhone(string? phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length != 13)
                return phone ?? string.Empty;
            return $"+20 {phone.Substring(3, 3)} {phone.Substring(6, 3)} {phone[^4..]}";
        }

        // name@example.com -> n•••@example.com
        public static string MaskEmail(string? email)
        {
            if (string.IsNullOrEmpty(email))
                return string.Empty;
            var at = email.IndexOf('@');
            if (at <= 1)
                return email;
            return email[0] + "•••" + email[at..];
        }

        public static bool LooksLikeEmail(string input) => input.Contains('@');
    }
}
