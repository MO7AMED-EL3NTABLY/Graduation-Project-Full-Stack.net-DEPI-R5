using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Chat;
using MVC.DAL.Entities.Locations;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Accounts
{
    // بروفايل الدكتور - جدول DoctorProfiles
    // DoctorId هو PK و FK لـ AspNetUsers.Id في نفس الوقت (Shared PK)
    public class DoctorProfile
    {
        public string DoctorId { get; set; } = null!; // نفس Id بتاع المستخدم

        public Guid SpecialtyId { get; set; } // التخصص (بيتغير بموافقة الأدمن بس)

        public string LicenseNumber { get; set; } = null!; // رقم الرخصة (بيتغير بموافقة الأدمن بس)

        public string University { get; set; } = null!; // الجامعة (بتتغير بموافقة الأدمن بس)

        public int GraduationYear { get; set; } // سنة التخرج (بتتغير بموافقة الأدمن بس)

        public int YearsOfExperience { get; set; } // سنين الخبرة - الدكتور بيدخلها بإيده

        public string? Bio { get; set; } // نبذة عن الدكتور (اختياري)

        public ApprovalStatus ApprovalStatus { get; set; } // حالة موافقة الأدمن على الدكتور

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Specialty Specialty { get; set; } = null!;
        public ICollection<DoctorWorkExperience> WorkExperiences { get; set; } = new List<DoctorWorkExperience>();
        public ICollection<DoctorSurgery> Surgeries { get; set; } = new List<DoctorSurgery>();
        public ICollection<DoctorDocument> Documents { get; set; } = new List<DoctorDocument>();
        public ICollection<DoctorProfileChangeRequest> ChangeRequests { get; set; } = new List<DoctorProfileChangeRequest>();
        public ICollection<DoctorLocation> DoctorLocations { get; set; } = new List<DoctorLocation>();
        public ICollection<VacationDay> VacationDays { get; set; } = new List<VacationDay>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<PatientCondition> CreatedConditions { get; set; } = new List<PatientCondition>(); // الأمراض المزمنة اللي الدكتور ده سجلها
        public ICollection<ChatThread> ChatThreads { get; set; } = new List<ChatThread>();
    }
}
