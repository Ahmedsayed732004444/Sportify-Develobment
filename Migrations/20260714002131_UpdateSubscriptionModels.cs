using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportiva.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubscriptionModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionPayment_ClubSubscriptions_ClubSubscriptionId",
                table: "SubscriptionPayment");

            migrationBuilder.DropIndex(
                name: "IX_ClubSubscriptions_ClubId_EndDate",
                table: "ClubSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPayment",
                table: "SubscriptionPayment");

            migrationBuilder.RenameTable(
                name: "SubscriptionPayment",
                newName: "SubscriptionPayments");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionPayment_ClubSubscriptionId",
                table: "SubscriptionPayments",
                newName: "IX_SubscriptionPayments_ClubSubscriptionId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "ClubSubscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ClubSubscriptions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ClubSubscriptions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "ClubSubscriptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ClubSubscriptions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ClubSubscriptions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPayments",
                table: "SubscriptionPayments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_CourtId_Day_StartTime",
                table: "TimeSlots",
                columns: new[] { "CourtId", "Day", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClubSubscriptions_ClubId_Status",
                table: "ClubSubscriptions",
                columns: new[] { "ClubId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ClubSubscriptions_UserId_ClubId_Status",
                table: "ClubSubscriptions",
                columns: new[] { "UserId", "ClubId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ClubSubscriptions_UserId_Status",
                table: "ClubSubscriptions",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId",
                unique: true,
                filter: "[Status] IN ('Pending', 'Confirmed')");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubSubscriptions_AspNetUsers_UserId",
                table: "ClubSubscriptions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionPayments_ClubSubscriptions_ClubSubscriptionId",
                table: "SubscriptionPayments",
                column: "ClubSubscriptionId",
                principalTable: "ClubSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubSubscriptions_AspNetUsers_UserId",
                table: "ClubSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionPayments_ClubSubscriptions_ClubSubscriptionId",
                table: "SubscriptionPayments");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_CourtId_Day_StartTime",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_ClubSubscriptions_ClubId_Status",
                table: "ClubSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_ClubSubscriptions_UserId_ClubId_Status",
                table: "ClubSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_ClubSubscriptions_UserId_Status",
                table: "ClubSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPayments",
                table: "SubscriptionPayments");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "ClubSubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ClubSubscriptions");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ClubSubscriptions");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "ClubSubscriptions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ClubSubscriptions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ClubSubscriptions");

            migrationBuilder.RenameTable(
                name: "SubscriptionPayments",
                newName: "SubscriptionPayment");

            migrationBuilder.RenameIndex(
                name: "IX_SubscriptionPayments_ClubSubscriptionId",
                table: "SubscriptionPayment",
                newName: "IX_SubscriptionPayment_ClubSubscriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPayment",
                table: "SubscriptionPayment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClubSubscriptions_ClubId_EndDate",
                table: "ClubSubscriptions",
                columns: new[] { "ClubId", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionPayment_ClubSubscriptions_ClubSubscriptionId",
                table: "SubscriptionPayment",
                column: "ClubSubscriptionId",
                principalTable: "ClubSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
