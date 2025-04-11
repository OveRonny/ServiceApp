namespace serviceApp.Server.Entities;

public class ConsumptionRecord
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;

    public decimal DieselAdded { get; set; }
    public decimal DieselPricePerLiter { get; set; }

    public int MileageHistoryId { get; set; }
    public MileageHistory? MileageHistory { get; set; }
}
