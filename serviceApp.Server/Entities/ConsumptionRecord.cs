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
    public Guid FamilyId { get; set; }

    public decimal TotalCost => DieselAdded * DieselPricePerLiter;

    public decimal? DieselConsumption
    {
        get
        {
            if (MileageHistory?.Vehicle == null) return null;

            // Hent forrige MileageHistory for kjøretøyet
            var previousMileage = MileageHistory.Vehicle.MileageHistories
                .Where(m => m.RecordedDate < MileageHistory.RecordedDate)
                .OrderByDescending(m => m.RecordedDate)
                .FirstOrDefault();

            if (previousMileage == null || MileageHistory.Mileage <= previousMileage.Mileage)
            {
                return null;
            }

            return DieselAdded / (MileageHistory.Mileage - previousMileage.Mileage);
        }
    }
}
