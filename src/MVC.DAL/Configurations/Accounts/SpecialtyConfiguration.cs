using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Configurations.Accounts
{
    // Specialties table
    public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
    {
        public void Configure(EntityTypeBuilder<Specialty> builder)
        {
            builder.ToTable("Specialties");

            builder.HasKey(s => s.SpecialtyId);
        }
    }
}
