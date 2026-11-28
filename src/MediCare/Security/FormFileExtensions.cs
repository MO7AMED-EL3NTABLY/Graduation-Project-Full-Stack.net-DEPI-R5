using MVC.BLL.Common;

namespace MVC.PL.Security
{
    // Converts IFormFile to FileUpload so the BLL doesn't depend on ASP.NET
    public static class FormFileExtensions
    {
        // Largest size we accept in memory (documents 10 MB) - the BLL checks the real limit for each type
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
            // If it's over the limit we only pass the length, and the BLL rejects it without reading the whole file
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
