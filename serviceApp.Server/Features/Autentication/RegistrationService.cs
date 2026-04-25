using Microsoft.AspNetCore.Identity;
using serviceApp.Server.Features.Emails;
using System.Net;

namespace serviceApp.Server.Features.Autentication;

public sealed class RegistrationService(
    UserManager<ApplicationUser> userManager,
    ISmtpEmailSender emailSender,
    IConfiguration config,
    ILogger<RegistrationService> logger) : IRegistrationService
{
    private readonly UserManager<ApplicationUser> _users = userManager;
    private readonly ISmtpEmailSender _email = emailSender;
    private readonly IConfiguration _config = config;
    private readonly ILogger<RegistrationService> _logger = logger;

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
        //_ = await _users.AddToRoleAsync(user, Roles.FamilyOwner);

        // Generate token in-scope, then fire email in background
        var token = await _users.GenerateEmailConfirmationTokenAsync(user);
        TrySendConfirmationEmail(user, token);

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
        if (user is null || user.EmailConfirmed) return (true, null); // don't leak existence

        var token = await _users.GenerateEmailConfirmationTokenAsync(user);
        var link = BuildConfirmLink(user.Id, token);
        return await SendConfirmationEmailAsync(user.Email!, link, ct);
    }

    private void TrySendConfirmationEmail(ApplicationUser user, string token)
    {
        var userId = user.Id;
        var email = user.Email!;
        var link = BuildConfirmLink(userId, token);
        _ = Task.Run(async () =>
        {
            try
            {
                var (ok, error) = await SendConfirmationEmailAsync(email, link, CancellationToken.None);
                if (!ok && !string.IsNullOrWhiteSpace(error))
                    _logger.LogWarning("Failed to send confirmation email to {Email}: {Error}", email, error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when sending confirmation email to {Email}", email);
            }
        });
    }

    private async Task<(bool ok, string? error)> SendConfirmationEmailAsync(string email, string link, CancellationToken ct)
    {
        var html = $$"""
                 <p>Hei!</p>
                 <p>Bekreft e-post for Progorb-kontoen din ved å klikke:
                    <a href="{{link}}" target="_self" rel="noopener">Bekreft e-post</a>
                 </p>
                 """;

        try
        {
            await _email.SendAsync(email, "Bekreft e-post", html, ct);
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kunne ikke sende bekreftelsesepost til {Email}", email);
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