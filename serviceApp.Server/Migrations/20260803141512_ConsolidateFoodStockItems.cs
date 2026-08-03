using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateFoodStockItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                WITH Aggregates AS (
                    SELECT FamilyId, FoodProductId,
                           MIN(Id) AS KeepId,
                           SUM(Quantity) AS TotalQuantity,
                           MAX(MinimumQuantity) AS MinimumQuantity,
                           MIN(BestBeforeDate) AS BestBeforeDate,
                           MAX(PurchasedDate) AS PurchasedDate
                    FROM FoodStockItems
                    GROUP BY FamilyId, FoodProductId
                    HAVING COUNT(*) > 1
                )
                UPDATE target
                SET Quantity = aggregate.TotalQuantity,
                    MinimumQuantity = aggregate.MinimumQuantity,
                    BestBeforeDate = aggregate.BestBeforeDate,
                    PurchasedDate = aggregate.PurchasedDate,
                    UpdatedAt = SYSUTCDATETIME()
                FROM FoodStockItems AS target
                INNER JOIN Aggregates AS aggregate ON target.Id = aggregate.KeepId;

                WITH Ranked AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (
                               PARTITION BY FamilyId, FoodProductId
                               ORDER BY Id) AS RowNumber
                    FROM FoodStockItems
                )
                DELETE target
                FROM FoodStockItems AS target
                INNER JOIN Ranked AS ranked ON target.Id = ranked.Id
                WHERE ranked.RowNumber > 1;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_FoodStockItems_FamilyId_FoodProductId",
                table: "FoodStockItems",
                columns: new[] { "FamilyId", "FoodProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoodStockItems_FamilyId_FoodProductId",
                table: "FoodStockItems");
        }
    }
}
