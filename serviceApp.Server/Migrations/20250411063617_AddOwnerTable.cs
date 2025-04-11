using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class AddOwnerTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "OwnerId",
            table: "Vehicles",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "Owner",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FirstName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                LastName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                PhoneNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Email = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Address = table.Column<string>(type: "nvarchar(50)", nullable: false),
                PostalCode = table.Column<string>(type: "nvarchar(50)", nullable: false),
                City = table.Column<string>(type: "nvarchar(50)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Owner", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_OwnerId",
            table: "Vehicles",
            column: "OwnerId");

        migrationBuilder.AddForeignKey(
            name: "FK_Vehicles_Owner_OwnerId",
            table: "Vehicles",
            column: "OwnerId",
            principalTable: "Owner",
            principalColumn: "Id",
            onDelete: ReferentialAction.NoAction);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Vehicles_Owner_OwnerId",
            table: "Vehicles");

        migrationBuilder.DropTable(
            name: "Owner");

        migrationBuilder.DropIndex(
            name: "IX_Vehicles_OwnerId",
            table: "Vehicles");

        migrationBuilder.DropColumn(
            name: "OwnerId",
            table: "Vehicles");
    }
}
