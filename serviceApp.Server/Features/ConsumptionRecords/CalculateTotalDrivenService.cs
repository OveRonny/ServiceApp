namespace serviceApp.Server.Features.ConsumptionRecords;


public class CalculateTotalDrivenService
{

    public int CalculateTotalDriven(
        List<ConsumptionRecord> allRecords,
        DateTime? startDate,
        DateTime? endExclusive)
    {
        if (allRecords == null || allRecords.Count == 0)
            return 0;

        // Sorter alle records på dato
        var orderedAll = allRecords
            .OrderBy(c => c.Date)
            .ThenBy(c => c.Id)
            .ToList();

        // Records innenfor perioden
        var periodRecords = orderedAll
            .Where(c => (!startDate.HasValue || c.Date >= startDate.Value) &&
                        (!endExclusive.HasValue || c.Date < endExclusive.Value))
            .ToList();

        if (!periodRecords.Any())
            return 0;

        var firstInPeriod = periodRecords.First();
        var lastInPeriod = periodRecords.Last();

        // Finn forrige record før start, hvis det finnes
        var previousBeforeStart = orderedAll
            .Where(c => startDate.HasValue && c.Date < startDate.Value)
            .OrderByDescending(c => c.Date)
            .FirstOrDefault();

        int startMileage = previousBeforeStart?.MileageHistory?.Mileage ?? firstInPeriod.MileageHistory?.Mileage ?? 0;
        int endMileage = lastInPeriod.MileageHistory?.Mileage ?? 0;

        return Math.Max(0, endMileage - startMileage);
    }
}