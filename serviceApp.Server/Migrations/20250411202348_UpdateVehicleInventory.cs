using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class UpdateVehicleInventory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Parts_VehicleInventories_VehicleInventoryId",
            table: "Parts");

        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "VehicleInventories",
            type: "nvarchar(200)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AlterColumn<int>(
            name: "VehicleInventoryId",
            table: "Parts",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddForeignKey(
            name: "FK_Parts_VehicleInventories_VehicleInventoryId",
            table: "Parts",
            column: "VehicleInventoryId",
            principalTable: "VehicleInventories",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Parts_VehicleInventories_VehicleInventoryId",
            table: "Parts");

        migrationBuilder.DropColumn(
            name: "Description",
            table: "VehicleInventories");

        migrationBuilder.AlterColumn<int>(
            name: "VehicleInventoryId",
            table: "Parts",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Parts_VehicleInventories_VehicleInventoryId",
            table: "Parts",
            column: "VehicleInventoryId",
            principalTable: "VehicleInventories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
