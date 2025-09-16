using Microsoft.AspNetCore.Identity;
using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Startup;

public static class AuthenticationSetup
{
    public static IServiceCollection AddAuthenticationSetup(this IServiceCollection services, IConfiguration configuration)
    {
        // Identity + EF stores + minimal API endpoints (/register, /login, /refresh, /logout)
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                // Optional password rules:
                // options.Password.RequireNonAlphanumeric = false;
                // options.Password.RequireUppercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        // Bearer tokens + Authorization
        services.AddAuthentication()
                .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthorization();

        // Current user helpers / HttpContext
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();

        // If you later switch to claims transformation for FamilyId,
        // register IClaimsTransformation here as well.

        return services;
    }
}
