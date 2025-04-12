namespace ServiceApp.UI.Models;

public class VehicleModel
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int OwnerId { get; set; }

    public string? FirstName { get; set; } = string.Empty;

    public DateTime DateCreated { get; set; } = DateTime.Now;

    public OwnerModel? Owner { get; set; }

    public ICollection<MileageHistoryModel> MileageHistories { get; set; } = new List<MileageHistoryModel>();
}
