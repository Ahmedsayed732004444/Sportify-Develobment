using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportiva.Migrations
{
    /// <inheritdoc />
    public partial class newmmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "MembershipUpgrades",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "MembershipUpgrades");
        }
    }
}
