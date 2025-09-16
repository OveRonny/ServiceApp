namespace serviceApp.Server.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;

    public Owner? Owner { get; set; }
    public string UserId { get; set; } = default!;
    public Guid FamilyId { get; set; }
    public ICollection<MileageHistory> MileageHistories { get; set; } = new List<MileageHistory>();
}
