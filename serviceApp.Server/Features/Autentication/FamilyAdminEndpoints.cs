using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace serviceApp.Server.Features.Autentication;

public static class FamilyAdminEndpoints
{
    public record MemberDto(string Id, string Email, bool IsAdmin);

    public static void MapFamilyAdmin(this WebApplication app)
    {
        // List members in my family (Admin only)
        app.MapGet("/api/family/members", [Authorize(Roles = "Admin")] async (
            ClaimsPrincipal principal,
            UserManager<ApplicationUser> users) =>
        {
            var meId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (meId is null) return Results.Unauthorized();
            var me = await users.FindByIdAsync(meId);
            if (me is null) return Results.Unauthorized();

            // All users sharing my FamilyId
            var familyId = me.FamilyId;
            var familyMembers = await users.Users
                .Where(u => u.FamilyId == familyId)
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            // Get admins in one pass
            var admins = await users.GetUsersInRoleAsync("Admin");
            var adminIds = admins.Where(a => a.FamilyId == familyId).Select(a => a.Id).ToHashSet();

            var result = familyMembers
                .Select(u => new MemberDto(u.Id, u.Email ?? "", adminIds.Contains(u.Id)))
                .ToList();

            return Results.Ok(result);
        });

        // Promote to Admin (Admin only)
        app.MapPost("/api/family/members/{userId}/promote", [Authorize(Roles = "Admin")] async (
            ClaimsPrincipal principal,
            string userId,
            UserManager<ApplicationUser> users) =>
        {
            var meId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (meId is null) return Results.Unauthorized();
            var me = await users.FindByIdAsync(meId);
            if (me is null) return Results.Unauthorized();

            var target = await users.FindByIdAsync(userId);
            if (target is null) return Results.NotFound();

            // Must be same family
            if (target.FamilyId != me.FamilyId) return Results.Forbid();

            if (!await users.IsInRoleAsync(target, "Admin"))
                await users.AddToRoleAsync(target, "Admin");

            return Results.Ok();
        });

        // Demote from Admin (Admin only, keep at least one admin)
        app.MapPost("/api/family/members/{userId}/demote", [Authorize(Roles = "Admin")] async (
            ClaimsPrincipal principal,
            string userId,
            UserManager<ApplicationUser> users) =>
        {
            var meId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (meId is null) return Results.Unauthorized();
            var me = await users.FindByIdAsync(meId);
            if (me is null) return Results.Unauthorized();

            var target = await users.FindByIdAsync(userId);
            if (target is null) return Results.NotFound();

            if (target.FamilyId != me.FamilyId) return Results.Forbid();

            // Don’t remove the last admin in the family
            var admins = await users.GetUsersInRoleAsync("Admin");
            var familyAdmins = admins.Where(a => a.FamilyId == me.FamilyId).ToList();
            var targetIsAdmin = familyAdmins.Any(a => a.Id == target.Id);

            if (!targetIsAdmin) return Results.Ok(); // nothing to do
            if (familyAdmins.Count <= 1) return Results.BadRequest("Family must have at least one admin.");

            await users.RemoveFromRoleAsync(target, "Admin");
            return Results.Ok();
        });
    }
}
