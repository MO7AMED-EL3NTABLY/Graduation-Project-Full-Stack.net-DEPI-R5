using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Prescriptions
{
    // ملف طبي للمريض (نتيجة تحليل/أشعة أو تقرير قديم) - جدول MedicalDocuments
    // كل صف = ملف واحد، ومفيش نص مستخرج أو ناتج AI بيتخزن
    public class MedicalDocument
    {
        public Guid MedicalDocumentId { get; set; }

        public string PatientId { get; set; } = null!; // FK لـ PatientProfiles

        public Guid? ScanOrderId { get; set; } // FK لـ ScanOrders - null للتقارير القديمة من غير طلب

        public string StorageKey { get; set; } = null!; // مفتاح الملف (مش رابط عام)

        public string OriginalFileName { get; set; } = null!; // اسم الملف الأصلي

        public string ContentType { get; set; } = null!; // نوع الملف

        public long FileSizeBytes { get; set; } // حجم الملف بالبايت

        public DateTimeOffset UploadedAtUtc { get; set; } // وقت الرفع

        // Navigation properties
        public PatientProfile Patient { get; set; } = null!;
        public ScanOrder? ScanOrder { get; set; }
    }
}
