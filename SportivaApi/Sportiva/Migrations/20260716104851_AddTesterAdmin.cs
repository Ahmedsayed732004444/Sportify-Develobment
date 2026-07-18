using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportiva.Migrations
{
    /// <inheritdoc />
    public partial class AddTesterAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "IsDisabled", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0191b2c3-c4fc-752e-9d95-40b30fa7a9b7", 0, "0191a4b6-c4fc-752e-9d95-40b42a925b8f", new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@sportiva.com", true, "Tester", false, "Admin", false, null, "ADMIN@SPORTIVA.COM", "ADMIN@SPORTIVA.COM", "AQAAAAIAAYagAAAAEF+nP3oIur8ZNGiku2rjPpIZ0zeD9A6/kdsn0J6C2n7YJtRyMA2iHrvr5W+1D2i++w==", null, false, "55BF92C9EF0249CDA210D85D1A851BD0", false, "admin@sportiva.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0191a4b6-c4fc-752e-9d95-40b5e4e68054", "0191b2c3-c4fc-752e-9d95-40b30fa7a9b7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0191a4b6-c4fc-752e-9d95-40b5e4e68054", "0191b2c3-c4fc-752e-9d95-40b30fa7a9b7" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0191b2c3-c4fc-752e-9d95-40b30fa7a9b7");
        }
    }
}
