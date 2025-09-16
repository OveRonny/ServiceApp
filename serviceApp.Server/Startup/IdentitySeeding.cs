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
            try
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"Failed to create role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring role {role}: {ex.Message}");
                // Optionally: rethrow if you want the app to fail hard
            }
        }
    }
}

