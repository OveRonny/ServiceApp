using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class RemoveFamilyId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Suppliers_FamilyId",
            table: "Suppliers");

        migrationBuilder.DropIndex(
            name: "IX_ServiceTypes_FamilyId",
            table: "ServiceTypes");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "Suppliers");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "ServiceTypes");

        migrationBuilder.AlterColumn<Guid>(
            name: "FamilyId",
            table: "AspNetUsers",
            type: "uniqueidentifier",
            nullable: false,
            defaultValueSql: "NEWID()",
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_FamilyId",
            table: "AspNetUsers",
            column: "FamilyId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_FamilyId",
            table: "AspNetUsers");

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

        migrationBuilder.AlterColumn<Guid>(
            name: "FamilyId",
            table: "AspNetUsers",
            type: "uniqueidentifier",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldDefaultValueSql: "NEWID()");

        migrationBuilder.CreateIndex(
            name: "IX_Suppliers_FamilyId",
            table: "Suppliers",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceTypes_FamilyId",
            table: "ServiceTypes",
            column: "FamilyId");
    }
}
