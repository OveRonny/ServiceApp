namespace serviceApp.Server.Features.Autentication;

public interface IRegistrationService
{
    Task<(bool ok, string? error)> RegisterAsync(RegistrationEndpoints.RegisterRequest req, CancellationToken ct);
    Task<(bool ok, string? error)> ConfirmEmailAsync(RegistrationEndpoints.ConfirmEmailRequest req, CancellationToken ct);
    Task<(bool ok, string? error)> ResendConfirmationAsync(RegistrationEndpoints.ResendRequest req, CancellationToken ct);
}