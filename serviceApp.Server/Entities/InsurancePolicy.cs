namespace serviceApp.Server.Entities;

public class InsurancePolicy
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal AnnualPrice { get; set; }
    public int AnnualMileageLimit { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public DateTime RenewalDate { get; set; }
    public int StartingMileage { get; set; }

    public ICollection<InsuranceHistory> InsuranceHistories { get; set; } = new List<InsuranceHistory>();

    public int CalculateRemainingMileage()
    {
        if (Vehicle == null || Vehicle.MileageHistories == null || !Vehicle.MileageHistories.Any())
        {
            throw new InvalidOperationException("No mileage history available for the vehicle.");
        }

        // Get the latest mileage record
        var latestMileage = Vehicle.MileageHistories
            .OrderByDescending(m => m.RecordedDate)
            .FirstOrDefault();

        if (latestMileage == null)
        {
            throw new InvalidOperationException("No mileage history available for the vehicle.");
        }

        // Get the mileage at the start of the insurance period
        var startMileage = Vehicle.MileageHistories
            .Where(m => m.RecordedDate <= RenewalDate.AddYears(-1)) // Assuming annual renewal
            .OrderByDescending(m => m.RecordedDate)
            .FirstOrDefault()?.Mileage ?? 0;

        // Calculate total mileage driven
        var totalMileageDriven = latestMileage.Mileage - startMileage;

        // Calculate remaining mileage
        var remainingMileage = AnnualMileageLimit - totalMileageDriven;

        // Ensure remaining mileage is not negative
        return remainingMileage > 0 ? remainingMileage : 0;
    }
}
