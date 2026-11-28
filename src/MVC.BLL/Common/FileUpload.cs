namespace MVC.BLL.Common
{
    // A file uploaded from a form - the PL converts IFormFile to this class so the BLL doesn't depend on ASP.NET
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

    // A stored file ready to download from an endpoint protected by authorization
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
