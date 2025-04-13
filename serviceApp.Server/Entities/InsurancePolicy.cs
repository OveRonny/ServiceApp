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

    public bool IsActive { get; set; }
    public DateTime? EndDate { get; set; }

    public int CalculateRemainingMileage(IEnumerable<MileageHistory> mileageHistories)
    {
        // Find the latest odometer reading from the mileage history
        var latestMileageRecord = mileageHistories
            .Where(m => m.VehicleId == VehicleId && m.RecordedDate >= RenewalDate && m.RecordedDate <= (EndDate ?? DateTime.UtcNow))
            .OrderByDescending(m => m.RecordedDate)
            .FirstOrDefault();

        if (latestMileageRecord == null)
        {
            // If no mileage records exist, assume no mileage has been driven
            return AnnualMileageLimit;
        }

        // Calculate mileage driven during the policy period
        int mileageDriven = latestMileageRecord.Mileage - StartingMileage;

        // Calculate remaining mileage
        int remainingMileage = AnnualMileageLimit - mileageDriven;

        // Ensure the remaining mileage is not negative
        return Math.Max(remainingMileage, 0);
    }
}
