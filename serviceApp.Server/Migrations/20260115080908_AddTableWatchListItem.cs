using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations;

/// <inheritdoc />
public partial class AddTableWatchListItem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WatchlistItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                MediaItemId = table.Column<int>(type: "int", nullable: false),
                AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WatchlistItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_WatchlistItems_MediaItems_MediaItemId",
                    column: x => x.MediaItemId,
                    principalTable: "MediaItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_WatchlistItems_MediaItemId",
            table: "WatchlistItems",
            column: "MediaItemId");

        migrationBuilder.CreateIndex(
            name: "IX_WatchlistItems_UserId_MediaItemId",
            table: "WatchlistItems",
            columns: new[] { "UserId", "MediaItemId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "WatchlistItems");
    }
}
