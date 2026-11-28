using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC.DAL.Entities.Accounts;
using MVC.DAL.Entities.Booking;
using MVC.DAL.Entities.Chat;
using MVC.DAL.Entities.Locations;
using MVC.DAL.Entities.Notifications;
using MVC.DAL.Entities.Prescriptions;
using MVC.DAL.Entities.Support;

namespace MVC.DAL.Dbcontexts
{
    // The main DbContext of the project - inherits from IdentityDbContext for the Identity tables (AspNet*)
    public class MediCareDbContext : IdentityDbContext<ApplicationUser>
    {
        public MediCareDbContext(DbContextOptions<MediCareDbContext> options) : base(options)
        {
           
        }

        // 01 Accounts
        public DbSet<PatientProfile> PatientProfiles { get; set; } = null!;
        public DbSet<DoctorProfile> DoctorProfiles { get; set; } = null!;
        public DbSet<Specialty> Specialties { get; set; } = null!;
        public DbSet<DoctorWorkExperience> DoctorWorkExperiences { get; set; } = null!;
        public DbSet<DoctorSurgery> DoctorSurgeries { get; set; } = null!;
        public DbSet<DoctorDocument> DoctorDocuments { get; set; } = null!;
        public DbSet<DoctorProfileChangeRequest> DoctorProfileChangeRequests { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<PhoneVerificationCode> PhoneVerificationCodes { get; set; } = null!;

        // 02 Locations
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<DoctorLocation> DoctorLocations { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<ShiftDay> ShiftDays { get; set; } = null!;
        public DbSet<ShiftException> ShiftExceptions { get; set; } = null!;
        public DbSet<AdditionalShift> AdditionalShifts { get; set; } = null!;
        public DbSet<VacationDay> VacationDays { get; set; } = null!;

        // 03 Booking
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<BookingHold> BookingHolds { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        // 04 Prescriptions
        public DbSet<Prescription> Prescriptions { get; set; } = null!;
        public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; } = null!;
        public DbSet<PrescriptionMedicineTime> PrescriptionMedicineTimes { get; set; } = null!;
        public DbSet<MedicationDoseLog> MedicationDoseLogs { get; set; } = null!;
        public DbSet<PatientCondition> PatientConditions { get; set; } = null!;
        public DbSet<PrescriptionCondition> PrescriptionConditions { get; set; } = null!;
        public DbSet<ScanOrder> ScanOrders { get; set; } = null!;
        public DbSet<MedicalDocument> MedicalDocuments { get; set; } = null!;

        // 05 Chat
        public DbSet<ChatThread> ChatThreads { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        // 06 Notifications
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<NotificationPreference> NotificationPreferences { get; set; } = null!;
        public DbSet<Reminder> Reminders { get; set; } = null!;

        // 06 Support
        public DbSet<SupportConversation> SupportConversations { get; set; } = null!;
        public DbSet<SupportMessage> SupportMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Must come first so Identity configures its tables
            base.OnModelCreating(builder);

            // Applies every configuration in the Configurations folder automatically
            builder.ApplyConfigurationsFromAssembly(typeof(MediCareDbContext).Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // All enums are stored as English strings with length 50
            configurationBuilder.Properties<Enum>()
                                .HaveConversion<string>()
                                .HaveMaxLength(50);

            // All money is decimal(10,2) in Egyptian pounds
            configurationBuilder.Properties<decimal>()
                                .HavePrecision(10, 2);
        }
    }
}
