using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceYearsOfExperienceWithPracticeStartYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YearsOfExperience",
                table: "DoctorProfiles",
                newName: "PracticeStartYear");

            // D-03: the column held the number of years of experience; convert it to the practice start year
            migrationBuilder.Sql("UPDATE [DoctorProfiles] SET [PracticeStartYear] = YEAR(GETUTCDATE()) - [PracticeStartYear];");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convert the start year back to a number of years before the column gets its old name back
            migrationBuilder.Sql("UPDATE [DoctorProfiles] SET [PracticeStartYear] = YEAR(GETUTCDATE()) - [PracticeStartYear];");

            migrationBuilder.RenameColumn(
                name: "PracticeStartYear",
                table: "DoctorProfiles",
                newName: "YearsOfExperience");
        }
    }
}
