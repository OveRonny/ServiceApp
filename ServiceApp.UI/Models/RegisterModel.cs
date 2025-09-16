using System.ComponentModel.DataAnnotations;

namespace ServiceApp.UI.Models;

public class RegisterModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password), ErrorMessage = "Passordene er ikke like")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
