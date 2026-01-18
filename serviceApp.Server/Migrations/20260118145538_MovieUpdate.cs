using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace serviceApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class MovieUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WatchHistories_UserId_MediaItemId_SeasonId_EpisodeId",
                table: "WatchHistories");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_UserId_MediaItemId_SeasonId_EpisodeId",
                table: "WatchHistories",
                columns: new[] { "UserId", "MediaItemId", "SeasonId", "EpisodeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WatchHistories_UserId_MediaItemId_SeasonId_EpisodeId",
                table: "WatchHistories");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_UserId_MediaItemId_SeasonId_EpisodeId",
                table: "WatchHistories",
                columns: new[] { "UserId", "MediaItemId", "SeasonId", "EpisodeId" },
                unique: true,
                filter: "[SeasonId] IS NOT NULL AND [EpisodeId] IS NOT NULL");
        }
    }
}
