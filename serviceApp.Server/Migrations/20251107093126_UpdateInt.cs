using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class UpdateInt : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Unit",
            table: "VehicleInventories");

        migrationBuilder.AlterColumn<int>(
            name: "ReorderThreshold",
            table: "VehicleInventories",
            type: "int",
            precision: 18,
            scale: 2,
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2,
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "QuantityInStock",
            table: "VehicleInventories",
            type: "int",
            precision: 18,
            scale: 2,
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2,
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "ReorderThreshold",
            table: "VehicleInventories",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int",
            oldPrecision: 18,
            oldScale: 2,
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "QuantityInStock",
            table: "VehicleInventories",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int",
            oldPrecision: 18,
            oldScale: 2,
            oldNullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Unit",
            table: "VehicleInventories",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }
}
