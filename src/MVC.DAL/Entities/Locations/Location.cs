using MVC.DAL.Enums;

namespace MVC.DAL.Entities.Locations
{
    // Practice location (clinic or hospital) - Locations table
    // Each doctor has their own location even at the same address, and there are no coordinates in this version
    public class Location
    {
        public Guid LocationId { get; set; }

        public string Name { get; set; } = null!; // Location name

        public LocationType LocationType { get; set; } // Clinic / Hospital

        public string Governorate { get; set; } = null!; // Governorate

        public string City { get; set; } = null!; // City

        public string Address { get; set; } = null!; // Full address

        // Navigation properties
        public DoctorLocation? DoctorLocation { get; set; } // The location has at most one DoctorLocation
    }
}
