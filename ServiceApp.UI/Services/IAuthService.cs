
namespace ServiceApp.UI.Services;

public interface IAuthService
{
    Task<string?> GetAccessTokenAsync();
    Task<bool> LoginAsync(string email, string password, CancellationToken ct = default);
    Task LogoutAsync(CancellationToken ct = default);
    Task<bool> RegisterAsync(string email, string password, CancellationToken ct = default);
    Task<(bool ok, string? error)> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken ct = default);
    Task<(bool ok, string? error)> DeleteAccountAsync(string password, string confirmation, CancellationToken ct = default);
    Task<bool> ResendConfirmationEmailAsync(string email, CancellationToken ct = default);
}
