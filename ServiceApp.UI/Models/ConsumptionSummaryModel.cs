namespace ServiceApp.UI.Models;

public class ConsumptionSummaryModel
{
    public decimal? AvgConsumption { get; set; }
    public decimal? AveragePrice { get; set; }
    public int TotalDriven { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalDieselAdded { get; set; }
}
