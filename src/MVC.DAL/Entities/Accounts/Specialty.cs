namespace MVC.DAL.Entities.Accounts
{
    // Medical specialties - Specialties table
    public class Specialty
    {
        public Guid SpecialtyId { get; set; }

        public string NameAr { get; set; } = null!; // Arabic name

        public string NameEn { get; set; } = null!; // English name

        public bool IsActive { get; set; } // When turned off it can't be chosen anymore, but it stays with existing doctors

        public string? ImageStorageKey { get; set; } // Specialty card image key (optional)

        // Navigation properties
        public ICollection<DoctorProfile> Doctors { get; set; } = new List<DoctorProfile>();
    }
}
