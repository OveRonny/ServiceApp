namespace ServiceApp.UI.Models;

public class MileageHistoryModel
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int Mileage { get; set; }
    public int? Hours { get; set; }
    public DateTime RecordedDate { get; set; } = DateTime.Now;
    public VehicleModel? Vehicle { get; set; }

    public MileageType Type { get; set; }
    public enum MileageType
    {
        Forsikring,
        Forbruk,
        Service
    }
}
