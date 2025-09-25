using Microsoft.AspNetCore.Identity;

namespace serviceApp.Server.Features.Roles;

public static class RoleSeeder
{
    public static async Task SeedAsync(
         RoleManager<IdentityRole> roles,
         UserManager<ApplicationUser> users,
         IConfiguration cfg)
    {
        // Ensure all roles exist
        var all = new[] { Roles.Admin, Roles.FamilyOwner, Roles.FamilyGuest, Roles.OwnerAdmin };
        foreach (var r in all.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!await roles.RoleExistsAsync(r))
                await roles.CreateAsync(new IdentityRole(r));
        }

        // Read owner config
        var ownerEmail = cfg["Owner:Email"];
        var ownerPassword = cfg["Owner:Password"]; // optional in prod if you create owner manually

        if (string.IsNullOrWhiteSpace(ownerEmail))
            return;

        // Find or create owner user
        var owner = await users.FindByEmailAsync(ownerEmail);
        if (owner is null)
        {
            if (string.IsNullOrWhiteSpace(ownerPassword))
            {
                // No password provided -> skip creation (avoid creating an unusable user)
                return;
            }

            owner = new ApplicationUser
            {
                UserName = ownerEmail,
                Email = ownerEmail,
                EmailConfirmed = true,
                FamilyId = Guid.NewGuid()
            };

            var create = await users.CreateAsync(owner, ownerPassword);
            if (!create.Succeeded)
                return; // optionally log errors
        }

        var requiredRoles = new[] { Roles.OwnerAdmin, Roles.Admin };
        foreach (var role in requiredRoles)
        {
            if (!await users.IsInRoleAsync(owner, role))
                await users.AddToRoleAsync(owner, role);
        }
    }
}
