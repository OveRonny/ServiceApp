namespace serviceApp.Server.Entities;

public class WatchHistory
{
    public int Id { get; set; }
    public int MediaItemId { get; set; }
    public MediaItem MediaItem { get; set; } = null!;
    public DateTime? WatchDate { get; set; }
    public int? SeasonId { get; set; }
    public Season? Season { get; set; }

    public int? EpisodeId { get; set; }
    public Episode? Episode { get; set; }
    public int? TimeSpentMinutes { get; set; }
    public double Progress { get; set; }

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public bool? Liked { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
