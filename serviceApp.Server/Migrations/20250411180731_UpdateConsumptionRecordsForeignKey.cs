using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class UpdateConsumptionRecordsForeignKey : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Drop the existing foreign key
        migrationBuilder.DropForeignKey(
            name: "FK_ConsumptionRecords_MileageHistories_MileageHistoryId",
            table: "ConsumptionRecords");

        // Add the new foreign key with Cascade behavior
        migrationBuilder.AddForeignKey(
            name: "FK_ConsumptionRecords_MileageHistories_MileageHistoryId",
            table: "ConsumptionRecords",
            column: "MileageHistoryId",
            principalTable: "MileageHistories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Revert to the original foreign key with NoAction behavior
        migrationBuilder.DropForeignKey(
            name: "FK_ConsumptionRecords_MileageHistories_MileageHistoryId",
            table: "ConsumptionRecords");

        migrationBuilder.AddForeignKey(
            name: "FK_ConsumptionRecords_MileageHistories_MileageHistoryId",
            table: "ConsumptionRecords",
            column: "MileageHistoryId",
            principalTable: "MileageHistories",
            principalColumn: "Id",
            onDelete: ReferentialAction.NoAction);

    }
}
