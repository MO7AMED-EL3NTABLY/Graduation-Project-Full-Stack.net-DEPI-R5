using MVC.BLL.Common;

namespace MVC.BLL.Interfaces
{
    // Stores protected files (profile photos and doctor documents) outside wwwroot
    // The file is saved with a StorageKey and has no public link - downloads only through an endpoint protected by authorization
    public interface IFileStorage
    {
        Task<string> SaveAsync(FileUpload file, string folder);

        Stream? OpenRead(string storageKey);

        void Delete(string storageKey);
    }
}
