using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFrozenFoodDatesAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "FrozenDate",
                table: "FoodStockItems",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FrozenOneYearNotificationSentAt",
                table: "FoodStockItems",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FrozenTwoYearNotificationSentAt",
                table: "FoodStockItems",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodStockItems_FamilyId_FrozenDate",
                table: "FoodStockItems",
                columns: new[] { "FamilyId", "FrozenDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoodStockItems_FamilyId_FrozenDate",
                table: "FoodStockItems");

            migrationBuilder.DropColumn(
                name: "FrozenDate",
                table: "FoodStockItems");

            migrationBuilder.DropColumn(
                name: "FrozenOneYearNotificationSentAt",
                table: "FoodStockItems");

            migrationBuilder.DropColumn(
                name: "FrozenTwoYearNotificationSentAt",
                table: "FoodStockItems");
        }
    }
}
