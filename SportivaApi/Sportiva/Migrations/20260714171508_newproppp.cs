using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportiva.Migrations
{
    /// <inheritdoc />
    public partial class newproppp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TimeSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_CourtId_Day_StartTime",
                table: "TimeSlots",
                columns: new[] { "CourtId", "Day", "StartTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_CourtId_Day_StartTime",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TimeSlots");
        }
    }
}
