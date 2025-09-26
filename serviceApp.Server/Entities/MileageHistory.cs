namespace serviceApp.Server.Entities;

public class MileageHistory
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int Mileage { get; set; }
    public int? Hours { get; set; }
    public DateTime RecordedDate { get; set; } = DateTime.Now;
    public Vehicle? Vehicle { get; set; }
    public MileageType Type { get; set; }
    public Guid FamilyId { get; set; }

    public enum MileageType
    {
        Forsikring,
        Forbruk,
        Service,
        Kilometerstand
    }
}
