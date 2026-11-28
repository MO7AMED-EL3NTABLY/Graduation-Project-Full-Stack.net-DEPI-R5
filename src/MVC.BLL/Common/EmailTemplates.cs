using System.Net;

namespace MVC.BLL.Common
{
    // Shared HTML template for account emails (email confirmation, password reset, email change)
    // Inline styles and tables so it renders correctly in Gmail and Outlook
    public static class EmailTemplates
    {
        private const string Brand = "#0F8A7E";

        public static string ActionEmail(string fullName, string heading, string message, string buttonText, string link, string note)
        {
            var name = WebUtility.HtmlEncode(fullName);
            var href = WebUtility.HtmlEncode(link);

            return $@"<!DOCTYPE html>
<html lang=""en"">
<body style=""margin:0;padding:0;background:#F3F6F8;font-family:Segoe UI,Arial,sans-serif;"">
  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#F3F6F8;padding:32px 16px;"">
    <tr><td align=""center"">
      <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width:520px;background:#FFFFFF;border-radius:12px;overflow:hidden;"">
        <tr><td style=""background:{Brand};padding:20px 32px;color:#FFFFFF;font-size:22px;font-weight:700;"">MediCare</td></tr>
        <tr><td style=""padding:32px;color:#1F2933;"">
          <p style=""margin:0 0 8px;font-size:15px;"">Hi {name},</p>
          <h1 style=""margin:0 0 12px;font-size:22px;color:#111827;"">{heading}</h1>
          <p style=""margin:0 0 24px;font-size:15px;line-height:1.6;color:#4B5563;"">{message}</p>
          <table role=""presentation"" cellpadding=""0"" cellspacing=""0""><tr><td style=""border-radius:8px;background:{Brand};"">
            <a href=""{href}"" style=""display:inline-block;padding:12px 28px;color:#FFFFFF;font-size:15px;font-weight:600;text-decoration:none;"">{buttonText}</a>
          </td></tr></table>
          <p style=""margin:24px 0 0;font-size:13px;color:#6B7280;"">{note}</p>
        </td></tr>
        <tr><td style=""padding:16px 32px;background:#F9FAFB;font-size:12px;color:#9CA3AF;"">
          If the button doesn't work, copy this link into your browser:<br />
          <a href=""{href}"" style=""color:{Brand};word-break:break-all;"">{href}</a>
        </td></tr>
      </table>
      <p style=""margin:16px 0 0;font-size:12px;color:#9CA3AF;"">© MediCare</p>
    </td></tr>
  </table>
</body>
</html>";
        }
    }
}
