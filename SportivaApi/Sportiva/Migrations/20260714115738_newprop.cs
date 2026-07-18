using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportiva.Migrations
{
    /// <inheritdoc />
    public partial class newprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthlyPrice",
                table: "SubscriptionPlans",
                newName: "Price");

            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Courts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInDays",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Courts");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "SubscriptionPlans",
                newName: "MonthlyPrice");
        }
    }
}
