using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodStoreAndPriceHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodStores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodStores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodPurchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodProductId = table.Column<int>(type: "int", nullable: false),
                    FoodStoreId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PurchasedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodPurchases_FoodProducts_FoodProductId",
                        column: x => x.FoodProductId,
                        principalTable: "FoodProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodPurchases_FoodStores_FoodStoreId",
                        column: x => x.FoodStoreId,
                        principalTable: "FoodStores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodPurchases_FamilyId_FoodProductId_PurchasedDate",
                table: "FoodPurchases",
                columns: new[] { "FamilyId", "FoodProductId", "PurchasedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FoodPurchases_FoodProductId",
                table: "FoodPurchases",
                column: "FoodProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPurchases_FoodStoreId",
                table: "FoodPurchases",
                column: "FoodStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodStores_FamilyId_Name",
                table: "FoodStores",
                columns: new[] { "FamilyId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodPurchases");

            migrationBuilder.DropTable(
                name: "FoodStores");
        }
    }
}
