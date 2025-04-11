namespace serviceApp.Server.Entities;

public class InsuranceHistory
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal AnnualPrice { get; set; }
    public int AnnualMileageLimit { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Vehicle? Vehicle { get; set; }
}
