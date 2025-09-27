namespace ServiceApp.UI.Models;

public class CreateUserModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public Guid? FamilyId { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
    public bool CreateNewFamily { get; set; } = false;
}
