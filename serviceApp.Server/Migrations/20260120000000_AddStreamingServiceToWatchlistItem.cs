using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class AddStreamingServiceToWatchlistItem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "StreamingService",
            table: "WatchlistItems",
            type: "int",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "StreamingService",
            table: "WatchlistItems");
    }
}
