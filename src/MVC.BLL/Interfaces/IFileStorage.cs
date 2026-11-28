/*——- Omar Hussein ——-*/
using MVC.BLL.Common;

namespace MVC.BLL.Interfaces
{
    // تخزين الملفات المحمية (صور البروفايل ومستندات الدكاترة) برة wwwroot
    // الملف بيتحفظ بـ StorageKey، ومفيش رابط عام - التحميل من Endpoint عليه Authorization بس
    public interface IFileStorage
    {
        Task<string> SaveAsync(FileUpload file, string folder);

        Stream? OpenRead(string storageKey);

        void Delete(string storageKey);
    }
}
/*————*/
