using Microsoft.AspNetCore.Identity;

namespace serviceApp.Server.Features.Users;

public sealed class MyAccountEndpoints : IEndpointDefinition
{
    private const string OwnerEmail = "overonny@progorb.no";

    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/account").RequireAuthorization().WithTags("My account");
        group.MapPost("/password", ChangePasswordAsync);
        group.MapPost("/delete", DeleteAccountAsync);
    }

    private static async Task<IResult> ChangePasswordAsync(ChangeOwnPasswordRequest request,
        HttpContext http, UserManager<ApplicationUser> users)
    {
        var user = await users.GetUserAsync(http.User);
        if (user is null) return Results.Unauthorized();
        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            return Results.BadRequest("Nåværende og nytt passord er påkrevd.");

        var result = await users.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded) return IdentityError(result);
        await users.UpdateSecurityStampAsync(user);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteAccountAsync(DeleteOwnAccountRequest request,
        HttpContext http, UserManager<ApplicationUser> users, ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var user = await users.GetUserAsync(http.User);
        if (user is null) return Results.Unauthorized();
        if (string.Equals(user.Email, OwnerEmail, StringComparison.OrdinalIgnoreCase))
            return Results.BadRequest("Hovedadministratoren kan ikke slette sin egen konto.");
        if (!string.Equals(request.Confirmation, "SLETT", StringComparison.Ordinal))
            return Results.BadRequest("Skriv SLETT for å bekrefte.");
        if (!await users.CheckPasswordAsync(user, request.Password))
            return Results.BadRequest("Passordet er ikke riktig.");

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.WatchHistories.Where(x => x.UserId == user.Id).ExecuteDeleteAsync(cancellationToken);
        await db.WatchlistItems.Where(x => x.UserId == user.Id).ExecuteDeleteAsync(cancellationToken);
        await db.Vehicles.IgnoreQueryFilters().Where(x => x.UserId == user.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(v => v.UserId, string.Empty), cancellationToken);

        var result = await users.DeleteAsync(user);
        if (!result.Succeeded)
        {
            await transaction.RollbackAsync(cancellationToken);
            return IdentityError(result);
        }

        await transaction.CommitAsync(cancellationToken);
        return Results.NoContent();
    }

    private static IResult IdentityError(IdentityResult result) =>
        Results.BadRequest(string.Join(" ", result.Errors.Select(x => x.Description)));
}

public sealed record ChangeOwnPasswordRequest(string CurrentPassword, string NewPassword);
public sealed record DeleteOwnAccountRequest(string Password, string Confirmation);
