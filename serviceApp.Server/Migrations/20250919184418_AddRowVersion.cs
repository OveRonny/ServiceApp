using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class AddRowVersion : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "Cost",
            table: "VehicleInventories",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(9,2)",
            oldPrecision: 9,
            oldScale: 2);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            table: "VehicleInventories",
            type: "rowversion",
            rowVersion: true,
            nullable: false,
            defaultValue: new byte[0]);

        migrationBuilder.AlterColumn<decimal>(
            name: "Cost",
            table: "ServiceRecords",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(9,2)",
            oldPrecision: 9,
            oldScale: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "Price",
            table: "Parts",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(9,2)",
            oldPrecision: 9,
            oldScale: 2,
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "AnnualPrice",
            table: "InsurancePolicies",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(9,2)",
            oldPrecision: 9,
            oldScale: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "DieselPricePerLiter",
            table: "ConsumptionRecords",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(9,2)",
            oldPrecision: 9,
            oldScale: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "DieselAdded",
            table: "ConsumptionRecords",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(9,2)",
            oldPrecision: 9,
            oldScale: 2);

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_FamilyId",
            table: "Vehicles",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_VehicleInventories_FamilyId_VehicleId",
            table: "VehicleInventories",
            columns: new[] { "FamilyId", "VehicleId" });

        migrationBuilder.CreateIndex(
            name: "IX_Suppliers_FamilyId",
            table: "Suppliers",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceTypes_FamilyId",
            table: "ServiceTypes",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRecords_FamilyId_VehicleId",
            table: "ServiceRecords",
            columns: new[] { "FamilyId", "VehicleId" });

        migrationBuilder.CreateIndex(
            name: "IX_ServiceCompanies_FamilyId",
            table: "ServiceCompanies",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_Parts_FamilyId",
            table: "Parts",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_Owner_FamilyId",
            table: "Owner",
            column: "FamilyId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Vehicles_FamilyId",
            table: "Vehicles");

        migrationBuilder.DropIndex(
            name: "IX_VehicleInventories_FamilyId_VehicleId",
            table: "VehicleInventories");

        migrationBuilder.DropIndex(
            name: "IX_Suppliers_FamilyId",
            table: "Suppliers");

        migrationBuilder.DropIndex(
            name: "IX_ServiceTypes_FamilyId",
            table: "ServiceTypes");

        migrationBuilder.DropIndex(
            name: "IX_ServiceRecords_FamilyId_VehicleId",
            table: "ServiceRecords");

        migrationBuilder.DropIndex(
            name: "IX_ServiceCompanies_FamilyId",
            table: "ServiceCompanies");

        migrationBuilder.DropIndex(
            name: "IX_Parts_FamilyId",
            table: "Parts");

        migrationBuilder.DropIndex(
            name: "IX_Owner_FamilyId",
            table: "Owner");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            table: "VehicleInventories");

        migrationBuilder.AlterColumn<decimal>(
            name: "Cost",
            table: "VehicleInventories",
            type: "decimal(9,2)",
            precision: 9,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "Cost",
            table: "ServiceRecords",
            type: "decimal(9,2)",
            precision: 9,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "Price",
            table: "Parts",
            type: "decimal(9,2)",
            precision: 9,
            scale: 2,
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2,
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "AnnualPrice",
            table: "InsurancePolicies",
            type: "decimal(9,2)",
            precision: 9,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "DieselPricePerLiter",
            table: "ConsumptionRecords",
            type: "decimal(9,2)",
            precision: 9,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "DieselAdded",
            table: "ConsumptionRecords",
            type: "decimal(9,2)",
            precision: 9,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2);
    }
}
