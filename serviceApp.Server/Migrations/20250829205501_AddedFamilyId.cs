using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class AddedFamilyId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "Vehicles",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<string>(
            name: "UserId",
            table: "Vehicles",
            type: "nvarchar(50)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<Guid>(
            name: "FamilyId",
            table: "AspNetUsers",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "Vehicles");

        migrationBuilder.DropColumn(
            name: "UserId",
            table: "Vehicles");

        migrationBuilder.DropColumn(
            name: "FamilyId",
            table: "AspNetUsers");
    }
}
