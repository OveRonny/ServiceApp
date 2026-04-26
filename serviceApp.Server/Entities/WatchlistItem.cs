namespace serviceApp.Server.Entities;

public class WatchlistItem
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;
    public int MediaItemId { get; set; }

    public MediaItem MediaItem { get; set; } = null!;

    public DateTime AddedAt { get; set; }

    public StreamingService? StreamingService { get; set; }
}
