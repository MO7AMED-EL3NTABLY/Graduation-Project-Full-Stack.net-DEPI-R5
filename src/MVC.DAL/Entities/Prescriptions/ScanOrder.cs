using MVC.DAL.Entities.Notifications;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Prescriptions
{
    // طلب تحليل أو أشعة من الدكتور - جدول ScanOrders
    // مربوط بالروشتة مش بالحجز، وملوش Status
    public class ScanOrder
    {
        public Guid ScanOrderId { get; set; }

        public Guid PrescriptionId { get; set; } // FK لـ Prescriptions

        public ScanType ScanType { get; set; } // تحليل / أشعة

        public string ScanName { get; set; } = null!; // اسم التحليل أو الأشعة

        public string? Notes { get; set; } // ملاحظات (اختياري)

        public DateTimeOffset CreatedAtUtc { get; set; } // وقت الإنشاء

        // Navigation properties
        public Prescription Prescription { get; set; } = null!;
        public ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>(); // ملفات النتايج
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
