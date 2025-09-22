namespace ServiceApp.UI.Models;

public class ConsumptionRecordModel
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;

    public decimal DieselAdded { get; set; }
    public decimal DieselPricePerLiter { get; set; }

    public int MileageHistoryId { get; set; }
    public MileageHistoryModel? MileageHistory { get; set; }

    public decimal? DieselConsumption { get; set; }

    public int Mileage { get; set; }
    public int? Hours { get; set; }

    public decimal TotalCost { get; set; }

    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

}