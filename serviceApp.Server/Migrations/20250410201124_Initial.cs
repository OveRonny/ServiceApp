using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Vehicles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Make = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Model = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Year = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Color = table.Column<string>(type: "nvarchar(50)", nullable: false),
                LicensePlate = table.Column<string>(type: "nvarchar(50)", nullable: false),
                DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vehicles", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Vehicles");
    }
}
