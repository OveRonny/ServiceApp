using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class UpdateImageTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsPrimary",
            table: "ImageFiles",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsPrimary",
            table: "ImageFiles");
    }
}
