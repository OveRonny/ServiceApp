using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class AddCommentToWatchHistory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Comment",
            table: "WatchHistories",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Comment",
            table: "WatchHistories");
    }
}
