/*——- Omar Hussein ——-*/
using MVC.BLL.Common;

namespace MVC.PL.Security
{
    // بيحوّل IFormFile لـ FileUpload عشان الـ BLL ميعتمدش على ASP.NET
    public static class FormFileExtensions
    {
        // أكبر حجم بنقبله في الذاكرة (المستندات 10 MB) - الـ BLL بيتأكد من الحجم الحقيقي لكل نوع
        private const long MaxBufferedBytes = 10 * 1024 * 1024 + 1;

        public static async Task<FileUpload?> ToFileUploadAsync(this IFormFile? file)
        {
            if (file is null || file.Length == 0)
                return null;

            var buffer = new MemoryStream();
            if (file.Length <= MaxBufferedBytes)
            {
                await file.CopyToAsync(buffer);
                buffer.Position = 0;
            }
            // لو أكبر من الحد بنبعت الطول بس والـ BLL هيرفضه من غير ما نقرا الملف كله
            return new FileUpload(buffer, file.FileName, file.ContentType, file.Length);
        }

        public static async Task<List<FileUpload>> ToFileUploadsAsync(this IEnumerable<IFormFile>? files)
        {
            var list = new List<FileUpload>();
            if (files is null)
                return list;
            foreach (var file in files)
            {
                var upload = await file.ToFileUploadAsync();
                if (upload is not null)
                    list.Add(upload);
            }
            return list;
        }
    }
}
/*————*/
