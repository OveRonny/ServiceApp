using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class UpdateInsurance : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "TraficInsurancePrice",
            table: "InsurancePolicies",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TraficInsurancePrice",
            table: "InsurancePolicies");
    }
}
