using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class UpdateMediaItem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_MediaItems_AspNetUsers_ApplicationUserId",
            table: "MediaItems");

        migrationBuilder.DropIndex(
            name: "IX_MediaItems_ApplicationUserId",
            table: "MediaItems");

        migrationBuilder.DropColumn(
            name: "ApplicationUserId",
            table: "MediaItems");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ApplicationUserId",
            table: "MediaItems",
            type: "nvarchar(450)",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_MediaItems_ApplicationUserId",
            table: "MediaItems",
            column: "ApplicationUserId");

        migrationBuilder.AddForeignKey(
            name: "FK_MediaItems_AspNetUsers_ApplicationUserId",
            table: "MediaItems",
            column: "ApplicationUserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id");
    }
}
