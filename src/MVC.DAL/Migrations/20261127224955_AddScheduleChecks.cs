using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_VacationDays_DateRange",
                table: "VacationDays",
                sql: "[EndDate] >= [StartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ShiftExceptions_Times",
                table: "ShiftExceptions",
                sql: "([ExceptionType] = 'Blocked' AND [StartTime] IS NULL AND [EndTime] IS NULL) OR ([ExceptionType] = 'SpecialWorkingHours' AND [StartTime] IS NOT NULL AND [EndTime] IS NOT NULL AND [EndTime] > [StartTime])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DoctorProfileChangeRequests_Review",
                table: "DoctorProfileChangeRequests",
                sql: "([Status] = 'Pending' AND [ReviewedAtUtc] IS NULL AND [ReviewedByAdminId] IS NULL) OR ([Status] IN ('Approved','Rejected') AND [ReviewedAtUtc] IS NOT NULL AND [ReviewedByAdminId] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_VacationDays_DateRange",
                table: "VacationDays");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ShiftExceptions_Times",
                table: "ShiftExceptions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DoctorProfileChangeRequests_Review",
                table: "DoctorProfileChangeRequests");
        }
    }
}
