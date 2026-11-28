/*——- Omar Hussein ——-*/
using System.Text.RegularExpressions;
using MVC.BLL.Common;
using MVC.BLL.Interfaces;

namespace MVC.BLL.Services.Infrastructure
{
    // مكان تخزين الملفات على السيرفر - بيتحدد في Program.cs (فولدر App_Data برة wwwroot)
    public class FileStorageOptions
    {
        public string RootPath { get; set; } = string.Empty;
    }

    // تخزين محلي: الـ StorageKey شكله "folder/guid.ext" والاسم الأصلي مش بيدخل في المسار خالص
    public class LocalFileStorage : IFileStorage
    {
        private static readonly Regex KeyPattern = new(@"^[a-z\-]+/[0-9a-f]{32}\.(pdf|jpg|jpeg|png)$", RegexOptions.Compiled);
        private readonly string _root;

        public LocalFileStorage(FileStorageOptions options)
        {
            _root = Path.GetFullPath(options.RootPath);
        }

        public async Task<string> SaveAsync(FileUpload file, string folder)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var key = $"{folder}/{Guid.NewGuid():N}{ext}";
            if (!KeyPattern.IsMatch(key))
                throw new InvalidOperationException("Invalid storage folder or file type.");

            var fullPath = ToFullPath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            file.Content.Position = 0;
            await using var output = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
            await file.Content.CopyToAsync(output);
            return key;
        }

        public Stream? OpenRead(string storageKey)
        {
            if (!KeyPattern.IsMatch(storageKey))
                return null;
            var fullPath = ToFullPath(storageKey);
            return File.Exists(fullPath) ? new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read) : null;
        }

        public void Delete(string storageKey)
        {
            if (!KeyPattern.IsMatch(storageKey))
                return;
            var fullPath = ToFullPath(storageKey);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        private string ToFullPath(string key) => Path.Combine(_root, key.Replace('/', Path.DirectorySeparatorChar));
    }
}
/*————*/
