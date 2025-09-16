using Microsoft.AspNetCore.Identity;
using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Startup;

public static class IdentitySeeding
{
    public static async Task EnsureRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in new[] { Roles.Admin, Roles.FamilyOwner, Roles.FamilyGuest })
        {
            if (!await roleManager.RoleExistsAsync(role))
                _ = await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    public static async Task EnsureOwnersAdminAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roles.RoleExistsAsync(Roles.Admin))
            await roles.CreateAsync(new IdentityRole(Roles.Admin));

        var owners = config.GetSection("OwnerEmails").Get<string[]>() ?? Array.Empty<string>();
        foreach (var email in owners.Where(e => !string.IsNullOrWhiteSpace(e)))
        {
            var user = await users.FindByEmailAsync(email);
            if (user is null) continue; // Optionally create the user here if desired
            if (!await users.IsInRoleAsync(user, Roles.Admin))
                _ = await users.AddToRoleAsync(user, Roles.Admin);
        }
    }
}
