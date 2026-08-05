using Microsoft.AspNetCore.Identity;
using AppRoles = serviceApp.Server.Features.Roles.Roles;

namespace serviceApp.Server.Features.Users;

public sealed class AdminUserEndpoints : IEndpointDefinition
{
    private const string OwnerEmail = "overonny@progorb.no";

    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/admin/users")
            .RequireAuthorization(policy => policy.RequireRole(AppRoles.OwnerAdmin))
            .WithTags("Admin users");

        group.MapGet("/", GetUsersAsync);
        group.MapPut("/{id}", UpdateUserAsync);
        group.MapPost("/{id}/password", ResetPasswordAsync);
        group.MapPost("/{id}/active", SetActiveAsync);
    }

    private static async Task<IResult> GetUsersAsync(UserManager<ApplicationUser> users,
        CancellationToken cancellationToken)
    {
        var result = new List<AdminUserDto>();
        var allUsers = await users.Users.OrderBy(x => x.Email).ToListAsync(cancellationToken);
        foreach (var user in allUsers)
        {
            var roles = await users.GetRolesAsync(user);
            result.Add(ToDto(user, roles));
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateUserAsync(string id, UpdateAdminUserRequest request,
        UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles)
    {
        var user = await users.FindByIdAsync(id);
        if (user is null) return Results.NotFound();

        var email = request.Email.Trim();
        if (string.IsNullOrWhiteSpace(email)) return Results.BadRequest("E-post er påkrevd.");

        var protectedOwner = IsOwner(user);
        if (protectedOwner && !string.Equals(email, OwnerEmail, StringComparison.OrdinalIgnoreCase))
            return Results.BadRequest("E-postadressen til hovedadministratoren kan ikke endres.");

        var duplicate = await users.FindByEmailAsync(email);
        if (duplicate is not null && duplicate.Id != user.Id)
            return Results.BadRequest("E-postadressen er allerede i bruk.");

        var requestedRoles = request.Roles
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (protectedOwner)
        {
            if (!requestedRoles.Contains(AppRoles.OwnerAdmin, StringComparer.OrdinalIgnoreCase)) requestedRoles.Add(AppRoles.OwnerAdmin);
            if (!requestedRoles.Contains(AppRoles.Admin, StringComparer.OrdinalIgnoreCase)) requestedRoles.Add(AppRoles.Admin);
        }

        foreach (var role in requestedRoles)
            if (!await roles.RoleExistsAsync(role)) return Results.BadRequest($"Rollen '{role}' finnes ikke.");

        user.Email = email;
        user.UserName = email;
        user.EmailConfirmed = protectedOwner || request.EmailConfirmed;
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        var update = await users.UpdateAsync(user);
        if (!update.Succeeded) return IdentityError(update);

        var currentRoles = await users.GetRolesAsync(user);
        var remove = currentRoles.Except(requestedRoles, StringComparer.OrdinalIgnoreCase).ToArray();
        var add = requestedRoles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToArray();
        if (remove.Length > 0)
        {
            var removed = await users.RemoveFromRolesAsync(user, remove);
            if (!removed.Succeeded) return IdentityError(removed);
        }
        if (add.Length > 0)
        {
            var added = await users.AddToRolesAsync(user, add);
            if (!added.Succeeded) return IdentityError(added);
        }

        await users.UpdateSecurityStampAsync(user);
        return Results.Ok(ToDto(user, await users.GetRolesAsync(user)));
    }

    private static async Task<IResult> ResetPasswordAsync(string id, ResetAdminPasswordRequest request,
        UserManager<ApplicationUser> users)
    {
        var user = await users.FindByIdAsync(id);
        if (user is null) return Results.NotFound();
        if (string.IsNullOrWhiteSpace(request.NewPassword)) return Results.BadRequest("Nytt passord er påkrevd.");

        var token = await users.GeneratePasswordResetTokenAsync(user);
        var result = await users.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded) return IdentityError(result);
        await users.UpdateSecurityStampAsync(user);
        return Results.NoContent();
    }

    private static async Task<IResult> SetActiveAsync(string id, SetAdminUserActiveRequest request,
        UserManager<ApplicationUser> users)
    {
        var user = await users.FindByIdAsync(id);
        if (user is null) return Results.NotFound();
        if (!request.IsActive && IsOwner(user))
            return Results.BadRequest("Hovedadministratoren kan ikke deaktiveres.");

        user.LockoutEnabled = true;
        user.LockoutEnd = request.IsActive ? null : DateTimeOffset.MaxValue;
        var result = await users.UpdateAsync(user);
        if (!result.Succeeded) return IdentityError(result);
        await users.UpdateSecurityStampAsync(user);
        return Results.NoContent();
    }

    private static bool IsOwner(ApplicationUser user) =>
        string.Equals(user.Email, OwnerEmail, StringComparison.OrdinalIgnoreCase);

    private static AdminUserDto ToDto(ApplicationUser user, IEnumerable<string> roles) => new(
        user.Id, user.Email ?? string.Empty, user.PhoneNumber, user.EmailConfirmed,
        user.LockoutEnd is null || user.LockoutEnd <= DateTimeOffset.UtcNow,
        user.FamilyId, roles.OrderBy(x => x).ToArray(), IsOwner(user));

    private static IResult IdentityError(IdentityResult result) =>
        Results.BadRequest(string.Join(" ", result.Errors.Select(x => x.Description)));
}

public sealed record AdminUserDto(string Id, string Email, string? PhoneNumber, bool EmailConfirmed,
    bool IsActive, Guid FamilyId, string[] Roles, bool IsProtectedOwner);
public sealed record UpdateAdminUserRequest(string Email, string? PhoneNumber, bool EmailConfirmed, string[] Roles);
public sealed record ResetAdminPasswordRequest(string NewPassword);
public sealed record SetAdminUserActiveRequest(bool IsActive);
