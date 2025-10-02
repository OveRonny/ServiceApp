using System.ComponentModel.DataAnnotations;

namespace ServiceApp.UI.Models;

public class VehicleModel
{
    public int Id { get; set; }
    [Required]
    public string Make { get; set; } = string.Empty;
    [Required]
    public string Model { get; set; } = string.Empty;
    [Required]
    public int Year { get; set; }
    [Required]
    public string Color { get; set; } = string.Empty;
    [Required]
    public string LicensePlate { get; set; } = string.Empty;
    [Required]
    public int OwnerId { get; set; }

    public string? FirstName { get; set; } = string.Empty;

    public DateTime DateCreated { get; set; } = DateTime.Now;

    public OwnerModel? Owner { get; set; }

    public ICollection<MileageHistoryModel> MileageHistories { get; set; } = new List<MileageHistoryModel>();
}
