using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class UpdateServiceRecordTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "MileageHistoryId",
            table: "ServiceRecords",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_MileageHistoryId",
            table: "ServiceRecords",
            column: "MileageHistoryId");

        migrationBuilder.AddForeignKey(
            name: "FK_ServiceRecords_MileageHistories_MileageHistoryId",
            table: "ServiceRecords",
            column: "MileageHistoryId",
            principalTable: "MileageHistories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ServiceRecords_MileageHistories_MileageHistoryId",
            table: "ServiceRecords");

        migrationBuilder.DropIndex(
            name: "IX_ServiceRecords_MileageHistoryId",
            table: "ServiceRecords");

        migrationBuilder.DropColumn(
            name: "MileageHistoryId",
            table: "ServiceRecords");
    }
}
