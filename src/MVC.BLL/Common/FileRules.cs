/*——- Omar Hussein ——-*/
namespace MVC.BLL.Common
{
    // قواعد الملفات المسموحة: النوع والحجم + التأكد من أول بايتات الملف (مش الامتداد بس)
    public static class FileRules
    {
        public const long MaxPhotoBytes = 5 * 1024 * 1024;     // 5 MB لصورة البروفايل
        public const long MaxDocumentBytes = 10 * 1024 * 1024; // 10 MB لمستندات الدكتور

        private static readonly string[] PhotoExtensions = { ".jpg", ".jpeg", ".png" };
        private static readonly string[] DocumentExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };

        public static string? ValidatePhoto(FileUpload? file)
        {
            if (file is null || file.Length == 0)
                return "Profile photo is required.";
            return Validate(file, PhotoExtensions, MaxPhotoBytes, "Profile photo must be a JPG or PNG image up to 5 MB.");
        }

        public static string? ValidateDocument(FileUpload? file)
        {
            if (file is null || file.Length == 0)
                return "Please choose a file.";
            return Validate(file, DocumentExtensions, MaxDocumentBytes, "Documents must be PDF, JPG or PNG files up to 10 MB.");
        }

        public static string ContentTypeFor(string fileNameOrKey)
        {
            return Path.GetExtension(fileNameOrKey).ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
        }

        private static string? Validate(FileUpload file, string[] allowed, long maxBytes, string error)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext) || file.Length > maxBytes)
                return error;

            if (!SignatureMatches(file.Content, ext))
                return error;

            return null;
        }

        // بنقرا أول 4 بايت ونرجّع الـ stream لأوله
        private static bool SignatureMatches(Stream stream, string ext)
        {
            if (!stream.CanSeek)
                return false;

            var header = new byte[4];
            stream.Position = 0;
            var read = stream.Read(header, 0, header.Length);
            stream.Position = 0;
            if (read < 4)
                return false;

            return ext switch
            {
                ".pdf" => header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46, // %PDF
                ".png" => header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
                ".jpg" or ".jpeg" => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                _ => false
            };
        }
    }
}
/*————*/
