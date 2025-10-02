using ServiceApp.UI.Pages;
using System.ComponentModel.DataAnnotations;

namespace ServiceApp.UI.Models;

public class InsuranseModel
{
    public int Id { get; set; }
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    [Required]
    public decimal AnnualPrice { get; set; }
    [Required]
    public decimal TraficInsurancePrice { get; set; }
    [Required]
    public int AnnualMileageLimit { get; set; }
    [Required]
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    [Required]
    public DateTime RenewalDate { get; set; }
    [Required]
    public int StartingMileage { get; set; }

    public bool IsActive { get; set; }
    public DateTime? EndDate { get; set; }

    public int? RemainingMileage { get; set; }
    public decimal? TotalPrice { get; set; }
}
