using System.ComponentModel.DataAnnotations;

namespace ServiceApp.UI.Models;

public sealed record AdminUserModel(string Id, string Email, string? PhoneNumber, bool EmailConfirmed,
    bool IsActive, Guid FamilyId, string[] Roles, bool IsProtectedOwner);

public sealed class AdminUserEditModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
    public HashSet<string> Roles { get; set; } = [];
}

public sealed class AdminPasswordResetModel
{
    [Required, MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
    [Required, Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
