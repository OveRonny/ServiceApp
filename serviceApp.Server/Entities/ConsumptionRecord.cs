using System.ComponentModel.DataAnnotations.Schema;

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

    public decimal? DieselConsumption => CalculateDieselConsumption();

    [NotMapped]
    public int? DrivenKm { get; set; }

    public decimal? CalculateDieselConsumption()
    {
        if (MileageHistory?.Vehicle == null) return null;

        var previousMileage = MileageHistory.Vehicle.MileageHistories
        .Where(m => m.RecordedDate < MileageHistory.RecordedDate)
        .OrderByDescending(m => m.RecordedDate)
        .FirstOrDefault();

        if (previousMileage == null) return null;

        var diffUnits = MileageHistory.Mileage - previousMileage.Mileage;
        if (diffUnits <= 0) return null;

        // If Mileage is stored in 0.1 km units (e.g., 206 => 20.6 km), convert to km
        const decimal unitToKm = 0.1m; // change to 1m if already km, or 0.001m if meters
        var distanceKm = diffUnits * unitToKm;

        if (distanceKm <= 0) return null;

        return DieselAdded / distanceKm; // e.g., 15 / 20.6 = 0.728 L/km
    }

  
}
