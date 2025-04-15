using System.ComponentModel.DataAnnotations;

namespace ServiceApp.UI.Models;

public class SupplierModel
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string ContactEmail { get; set; } = string.Empty;
    [Required]
    public string ContactPhone { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]

    public string PostalCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
}
