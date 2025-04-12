using System.ComponentModel.DataAnnotations;

namespace ServiceApp.UI.Models;

public class OwnerModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Legg inn fornavn")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Legg inn etternavn")]
    public string LastName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Legg inn telefon nummer")]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Legg inn epost")]
    [EmailAddress(ErrorMessage = "Feil format på epost")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Legg inn adresse")]
    public string Address { get; set; } = string.Empty;
    [Required(ErrorMessage = "Legg inn post nummer")]
    public string PostalCode { get; set; } = string.Empty;
    [Required(ErrorMessage = "Legg inn sted")]
    public string City { get; set; } = string.Empty;
}
