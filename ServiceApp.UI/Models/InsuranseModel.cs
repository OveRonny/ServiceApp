using ServiceApp.UI.Pages;

namespace ServiceApp.UI.Models;

public class InsuranseModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal AnnualPrice { get; set; }
    public decimal TraficInsurancePrice { get; set; }
    public int AnnualMileageLimit { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public DateTime RenewalDate { get; set; }
    public int StartingMileage { get; set; }

    public bool IsActive { get; set; }
    public DateTime? EndDate { get; set; }

    public int? RemainingMileage { get; set; }
    public decimal? TotalPrice { get; set; }
}
