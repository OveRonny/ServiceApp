using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodStockBatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodStockBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodStockItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    BestBeforeDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FrozenDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PurchasedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodStockBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodStockBatches_FoodStockItems_FoodStockItemId",
                        column: x => x.FoodStockItemId,
                        principalTable: "FoodStockItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO [FoodStockBatches]
                    ([FamilyId], [FoodStockItemId], [Quantity], [BestBeforeDate], [FrozenDate], [PurchasedDate], [CreatedAt])
                SELECT [FamilyId], [Id], [Quantity], [BestBeforeDate], [FrozenDate], [PurchasedDate], [CreatedAt]
                FROM [FoodStockItems]
                WHERE [Quantity] > 0;
                """);
            migrationBuilder.CreateIndex(
                name: "IX_FoodStockBatches_FamilyId_FoodStockItemId_BestBeforeDate",
                table: "FoodStockBatches",
                columns: new[] { "FamilyId", "FoodStockItemId", "BestBeforeDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FoodStockBatches_FoodStockItemId",
                table: "FoodStockBatches",
                column: "FoodStockItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodStockBatches");
        }
    }
}
