using Microsoft.AspNetCore.Identity;
using serviceApp.Server.Features.Emails;
using System.Net;

namespace serviceApp.Server.Features.Autentication;

public sealed class RegistrationService(
    UserManager<ApplicationUser> userManager,
    ISmtpEmailSender emailSender,
    IConfiguration config) : IRegistrationService
{
    private readonly UserManager<ApplicationUser> _users = userManager;
    private readonly ISmtpEmailSender _email = emailSender;
    private readonly IConfiguration _config = config;

    public async Task<(bool ok, string? error)> RegisterAsync(RegistrationEndpoints.RegisterRequest req, CancellationToken ct)
    {
        var existing = await _users.FindByEmailAsync(req.Email);
        if (existing is not null) return (false, "E-post er allerede registrert.");

        // Create a new family for this user
        var user = new ApplicationUser
        {
            UserName = req.Email,
            Email = req.Email,
            EmailConfirmed = false,
            FamilyId = Guid.NewGuid()
        };

        var created = await _users.CreateAsync(user, req.Password);
        if (!created.Succeeded) return (false, string.Join("; ", created.Errors.Select(e => e.Description)));

        // Default: FamilyOwner
        _ = await _users.AddToRoleAsync(user, Roles.FamilyOwner);

        // Optional: platform admin by config
        var admins = _config.GetSection("OwnerEmails").Get<string[]>() ?? Array.Empty<string>();

        _ = await SendConfirmationEmailAsync(user, ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> ConfirmEmailAsync(RegistrationEndpoints.ConfirmEmailRequest req, CancellationToken ct)
    {
        var user = await _users.FindByIdAsync(req.UserId);
        if (user is null) return (false, "Ugyldig bruker.");
        if (user.EmailConfirmed) return (true, null);


        var result = await _users.ConfirmEmailAsync(user, req.Code);
        return result.Succeeded ? (true, null) : (false, "Ugyldig eller utløpt kode.");
    }

    public async Task<(bool ok, string? error)> ResendConfirmationAsync(RegistrationEndpoints.ResendRequest req, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(req.Email);
        if (user is null || user.EmailConfirmed) return (true, null); // don’t leak existence

        return await SendConfirmationEmailAsync(user, ct);
    }

    private async Task<(bool ok, string? error)> SendConfirmationEmailAsync(ApplicationUser user, CancellationToken ct)
    {
        var token = await _users.GenerateEmailConfirmationTokenAsync(user);
        var link = BuildConfirmLink(user.Id, token);
        var html = $$"""
                 <p>Hei!</p>
                 <p>Bekreft e-post for Progorb-kontoen din ved å klikke:
                    <a href="{{link}}" target="_self" rel="noopener">Bekreft e-post</a>
                 </p>
                 """;

        try
        {
            await _email.SendAsync(user.Email!, "Bekreft e-post", html, ct);
            return (true, null);
        }
        catch
        {
            // TODO: inject and log with ILogger<RegistrationService>
            return (false, "Kunne ikke sende bekreftelsesepost.");
        }
    }

    private string BuildConfirmLink(string userId, string token)
    {
        // Use production URL if configured, otherwise fallback to localhost
        var uiBase = _config["UiBaseUrl"] ?? "https://todo.progorb.no";

        // Encode user ID and token to be safe in URL
        var encodedUserId = WebUtility.UrlEncode(userId);
        var encodedToken = WebUtility.UrlEncode(token);

        // Build the full confirmation link
        return $"{uiBase}/confirm-email?userId={encodedUserId}&code={encodedToken}";
    }
}