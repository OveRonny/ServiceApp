using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class AddedFamilyIdToAll : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "VehicleInventories",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "Suppliers",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "ServiceTypes",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "ServiceRecords",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "ServiceCompanies",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "Parts",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "Owner",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "MileageHistories",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "InsurancePolicies",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "ConsumptionRecords",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "VehicleInventories");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "Suppliers");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "ServiceTypes");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "ServiceRecords");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "ServiceCompanies");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "Parts");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "Owner");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "MileageHistories");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "InsurancePolicies");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "ConsumptionRecords");
    }
}
