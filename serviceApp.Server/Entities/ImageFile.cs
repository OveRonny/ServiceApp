namespace serviceApp.Server.Entities;

public class ImageFile
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public ImageEntityType EntityType { get; set; }
    public int EntityId { get; set; }
    public DateTime UploadedAt { get; set; }
}

public enum ImageEntityType
{
    Vehicle,
    ServiceRecord
}
