using Microsoft.AspNetCore.Identity;

namespace serviceApp.Server.Data;

public class ApplicationUser : IdentityUser
{
    public Guid FamilyId { get; set; } = Guid.NewGuid();
}
