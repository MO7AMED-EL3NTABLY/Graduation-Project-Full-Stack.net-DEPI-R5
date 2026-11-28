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

            // D-03: العمود كان فيه عدد سنين الخبرة، نحوله لسنة بداية الممارسة
            migrationBuilder.Sql("UPDATE [DoctorProfiles] SET [PracticeStartYear] = YEAR(GETUTCDATE()) - [PracticeStartYear];");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // نرجّع سنة البداية لعدد سنين قبل ما العمود يرجع لاسمه القديم
            migrationBuilder.Sql("UPDATE [DoctorProfiles] SET [PracticeStartYear] = YEAR(GETUTCDATE()) - [PracticeStartYear];");

            migrationBuilder.RenameColumn(
                name: "PracticeStartYear",
                table: "DoctorProfiles",
                newName: "YearsOfExperience");
        }
    }
}
