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
    public int CalculateDrivenLastRecordToNow(List<ConsumptionRecord> consumptionRecords)
    {
        if (consumptionRecords == null || consumptionRecords.Count == 0)
            return 0;

        var orderedAll = consumptionRecords
           .OrderBy(c => c.Date)
           .ThenBy(c => c.Id)
           .ToList();

        // Try to find a property named "DrivenKm" on ConsumptionRecord to populate per-record values.
        var drivenProp = typeof(ConsumptionRecord).GetProperty("DrivenKm");

        int totalDriven = 0;
        int? previousMileage = null;

        foreach (var record in orderedAll)
        {
            // Use mileage from the record's MileageHistory if available
            var currentMileage = record.MileageHistory?.Mileage;

            int drivenForRecord = 0;

            if (currentMileage.HasValue)
            {
                if (previousMileage.HasValue)
                {
                    // normal case: previous known mileage exists => difference
                    drivenForRecord = Math.Max(0, currentMileage.Value - previousMileage.Value);
                }
                else
                {
                    // first known mileage in the ordered sequence -> no prior point within this list
                    // treat driven as 0 (alternatively could try to find an earlier external record)
                    drivenForRecord = 0;
                }

                // update previousMileage for next iteration
                previousMileage = currentMileage.Value;
            }
            else
            {
                // no mileage recorded on this record -> driven = 0, do not change previousMileage
                drivenForRecord = 0;
            }

            totalDriven += drivenForRecord;

            // If the model contains a DrivenKm property, populate it so UI can show it per record.
            if (drivenProp is not null && drivenProp.CanWrite)
            {
                // handle both int and int? typed properties
                if (drivenProp.PropertyType == typeof(int) || drivenProp.PropertyType == typeof(int?))
                {
                    drivenProp.SetValue(record, drivenForRecord);
                }
            }
        }

        return totalDriven;
    }
}