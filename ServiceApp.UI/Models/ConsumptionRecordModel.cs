namespace ServiceApp.UI.Models;

public class ConsumptionRecordModel
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public VehicleModel? Vehicle { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;

    public decimal DieselAdded { get; set; }
    public decimal DieselPricePerLiter { get; set; }

    public int MileageHistoryId { get; set; }
    public MileageHistoryModel? MileageHistory { get; set; }
}
