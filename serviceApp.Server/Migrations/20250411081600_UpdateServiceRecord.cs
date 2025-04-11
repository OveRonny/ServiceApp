using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class UpdateServiceRecord : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ServiceCompanyId",
            table: "ServiceRecords",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_ServiceCompanyId",
            table: "ServiceRecords",
            column: "ServiceCompanyId");

        migrationBuilder.AddForeignKey(
            name: "FK_ServiceRecords_ServiceCompanies_ServiceCompanyId",
            table: "ServiceRecords",
            column: "ServiceCompanyId",
            principalTable: "ServiceCompanies",
            principalColumn: "Id",
            onDelete: ReferentialAction.NoAction);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ServiceRecords_ServiceCompanies_ServiceCompanyId",
            table: "ServiceRecords");

        migrationBuilder.DropIndex(
            name: "IX_ServiceRecords_ServiceCompanyId",
            table: "ServiceRecords");

        migrationBuilder.DropColumn(
            name: "ServiceCompanyId",
            table: "ServiceRecords");
    }
}
