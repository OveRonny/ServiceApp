namespace ServiceApp.UI.Models;

public class Command
{
    public int TmdbId { get; set; }
    public bool MarkAsWatched { get; set; } = false;
    public bool? Liked { get; set; }
    public int? Rating { get; set; }
    public DateTime? Date { get; set; }
}
