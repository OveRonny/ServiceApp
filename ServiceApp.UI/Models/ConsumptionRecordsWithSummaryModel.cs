namespace ServiceApp.UI.Models;

public class ConsumptionRecordsWithSummaryModel
{
    public List<ConsumptionRecordModel> Records { get; set; } = new();
    public ConsumptionSummaryModel Summary { get; set; } = new();
}
