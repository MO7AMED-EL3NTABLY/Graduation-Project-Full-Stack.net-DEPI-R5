using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Chat;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Accounts
{
    // بروفايل المريض - جدول PatientProfiles
    // PatientId هو PK و FK لـ AspNetUsers.Id في نفس الوقت (Shared PK)
    public class PatientProfile
    {
        public string PatientId { get; set; } = null!; // نفس Id بتاع المستخدم

        public DateOnly DateOfBirth { get; set; } // تاريخ الميلاد (العمر بيتحسب ومش بيتخزن)

        public Gender Gender { get; set; } // ذكر / أنثى

        public bool ShareMedicalProfile { get; set; } // مشاركة الملف الطبي مع الدكاترة (default false)

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<BookingHold> BookingHolds { get; set; } = new List<BookingHold>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<PatientCondition> Conditions { get; set; } = new List<PatientCondition>();
        public ICollection<MedicalDocument> MedicalDocuments { get; set; } = new List<MedicalDocument>();
        public ICollection<ChatThread> ChatThreads { get; set; } = new List<ChatThread>();
    }
}
