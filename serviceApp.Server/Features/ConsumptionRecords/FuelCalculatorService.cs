namespace serviceApp.Server.Features.ConsumptionRecords;

public class FuelCalculatorService
{
    /// <summary>
    /// Calculates the average fuel consumption (liters per km) over a list of consumption records,
    /// only including records within the optional date range, and excluding the last fill-up.
    /// </summary>
    public decimal? CalculateAverageConsumption(
        List<ConsumptionRecord> records,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        if (records == null || records.Count < 2)
            return null;

        // 1. Filter by user-selected date range
        var filteredRecords = records
            .Where(r => (!startDate.HasValue || r.Date >= startDate.Value.Date) &&
                        (!endDate.HasValue || r.Date <= endDate.Value.Date))
            .OrderBy(r => r.Date)
            .ToList();

        if (filteredRecords.Count < 2)
            return null; // Not enough records to calculate consumption

        // 2. Exclude last fill-up in the range
        filteredRecords = filteredRecords.Take(filteredRecords.Count - 1).ToList();

        decimal totalLiters = 0;
        int totalDistance = 0;

        // 3. Sum total liters and total distance
        for (int i = 1; i < filteredRecords.Count; i++)
        {
            var prev = filteredRecords[i - 1];
            var curr = filteredRecords[i];

            if (curr.MileageHistory?.Mileage > prev.MileageHistory?.Mileage)
            {
                int distance = curr.MileageHistory.Mileage - prev.MileageHistory.Mileage;
                totalDistance += distance;
                totalLiters += curr.DieselAdded; // Fuel added at current fill-up
            }
        }

        return totalDistance > 0 ? totalLiters / totalDistance : (decimal?)null;
    }
    public decimal? CalculateAveragePer100Km(
        List<ConsumptionRecord> records,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var avg = CalculateAverageConsumption(records, startDate, endDate);
        return avg?.MultiplyBy100();
    }

    public decimal? CalculateAveragePrice(List<ConsumptionRecord> records, DateTime? startDate = null, DateTime? endDate = null)
    {
        var filteredRecords = records
            .Where(r => (!startDate.HasValue || r.Date >= startDate.Value.Date) &&
                        (!endDate.HasValue || r.Date <= endDate.Value.Date))
            .ToList();

        decimal totalLiters = filteredRecords.Sum(r => r.DieselAdded);

        return totalLiters > 0
            ? filteredRecords.Sum(r => r.DieselAdded * r.DieselPricePerLiter) / totalLiters
            : (decimal?)null;
    }

}


public static class DecimalExtensions
{
    public static decimal MultiplyBy100(this decimal value) => value * 10m;
}