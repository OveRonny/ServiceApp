using Microsoft.AspNetCore.Identity;

namespace serviceApp.Server.Features.Autentication;

public static class RegistrationEndpoints
{
    public record RegisterRequest(string Email, string Password);
    public record ConfirmEmailRequest(string UserId, string Code);
    public record ResendRequest(string Email);

    public static void MapRegistration(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").AllowAnonymous();

        group.MapPost("/register", async (IRegistrationService svc, RegisterRequest req, CancellationToken ct) =>
        {
            var (ok, error) = await svc.RegisterAsync(req, ct);
            return ok ? Results.Ok() : Results.BadRequest(error);
        });

        group.MapPost("/confirm-email", async (IRegistrationService svc, ConfirmEmailRequest req, CancellationToken ct) =>
        {
            var (ok, error) = await svc.ConfirmEmailAsync(req, ct);
            return ok ? Results.Ok() : Results.BadRequest(error);
        });

        group.MapPost("/resend-confirmation", async (IRegistrationService svc, ResendRequest req, CancellationToken ct) =>
        {
            var (ok, error) = await svc.ResendConfirmationAsync(req, ct);
            return ok ? Results.Ok() : Results.BadRequest(error);
        });

        app.MapGet("/api/auth/me", async (UserManager<ApplicationUser> users, HttpContext http) =>
        {
            var user = await users.GetUserAsync(http.User);
            if (user is null) return Results.Unauthorized();
            var roles = await users.GetRolesAsync(user);
            return Results.Ok(new { id = user.Id, email = user.Email, roles });
        })
        .RequireAuthorization();
    }
}
