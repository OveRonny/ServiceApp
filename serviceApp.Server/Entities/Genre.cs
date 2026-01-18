namespace serviceApp.Server.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<MediaItemGenre> MediaItemGenres { get; set; } = new List<MediaItemGenre>();
}
