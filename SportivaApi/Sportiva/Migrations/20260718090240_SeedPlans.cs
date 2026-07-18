using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sportiva.Migrations
{
    /// <inheritdoc />
    public partial class SeedPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "CreatedAt", "Description", "DurationInDays", "ExpiresAt", "IsActive", "IsDeleted", "MaxCourts", "Name", "Price" },
                values: new object[,]
                {
                    { "0191b2c3-c4fc-752e-9d95-40b30fa7a9b1", new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Perfect for local single-pitch venues. Monitor 1 club and host 1 active tournament.", 365, null, true, false, 1, "Basic Plan", 100m },
                    { "0191b2c3-c4fc-752e-9d95-40b30fa7a9b2", new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Perfect for growing clubs. Manage up to 2 clubs and host 3 active tournaments simultaneously.", 365, null, true, false, 3, "Premium Plan", 250m },
                    { "0191b2c3-c4fc-752e-9d95-40b30fa7a9b3", new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "For sports centers and large complexes. Manage up to 5 clubs and host 10 active tournaments.", 365, null, true, false, 10, "Elite Plan", 500m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: "0191b2c3-c4fc-752e-9d95-40b30fa7a9b1");

            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: "0191b2c3-c4fc-752e-9d95-40b30fa7a9b2");

            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: "0191b2c3-c4fc-752e-9d95-40b30fa7a9b3");
        }
    }
}
