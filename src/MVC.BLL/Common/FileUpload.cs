/*——- Omar Hussein ——-*/
namespace MVC.BLL.Common
{
    // ملف مرفوع من الفورم - الـ PL بيحوّل IFormFile للكلاس ده عشان الـ BLL ميعتمدش على ASP.NET
    public sealed class FileUpload
    {
        public FileUpload(Stream content, string fileName, string contentType, long length)
        {
            Content = content;
            FileName = fileName;
            ContentType = contentType;
            Length = length;
        }

        public Stream Content { get; }
        public string FileName { get; }
        public string ContentType { get; }
        public long Length { get; }
    }

    // ملف متخزن جاهز للتحميل من Endpoint عليه Authorization
    public sealed class StoredFile
    {
        public StoredFile(Stream content, string contentType, string fileName)
        {
            Content = content;
            ContentType = contentType;
            FileName = fileName;
        }

        public Stream Content { get; }
        public string ContentType { get; }
        public string FileName { get; }
    }
}
/*————*/
