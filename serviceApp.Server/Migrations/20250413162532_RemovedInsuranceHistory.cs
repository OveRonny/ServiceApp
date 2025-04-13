using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class RemovedInsuranceHistory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InsuranceHistories");

        migrationBuilder.AddColumn<DateTime>(
            name: "EndDate",
            table: "InsurancePolicies",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsActive",
            table: "InsurancePolicies",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "EndDate",
            table: "InsurancePolicies");

        migrationBuilder.DropColumn(
            name: "IsActive",
            table: "InsurancePolicies");

        migrationBuilder.CreateTable(
            name: "InsuranceHistories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                VehicleId = table.Column<int>(type: "int", nullable: false),
                AnnualMileageLimit = table.Column<int>(type: "int", nullable: false),
                AnnualPrice = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                InsurancePolicyId = table.Column<int>(type: "int", nullable: true),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InsuranceHistories", x => x.Id);
                table.ForeignKey(
                    name: "FK_InsuranceHistories_InsurancePolicies_InsurancePolicyId",
                    column: x => x.InsurancePolicyId,
                    principalTable: "InsurancePolicies",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_InsuranceHistories_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_InsuranceHistories_InsurancePolicyId",
            table: "InsuranceHistories",
            column: "InsurancePolicyId");

        migrationBuilder.CreateIndex(
            name: "IX_InsuranceHistories_VehicleId",
            table: "InsuranceHistories",
            column: "VehicleId");
    }
}
