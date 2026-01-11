namespace ServiceApp.UI.Models;

public class ImageModel
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public bool IsPrimary { get; set; }
}

public enum ImageEntityType
{
    Vehicle = 0,
    ServiceRecord = 1
}
