using Blazored.Toast.Services;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ServiceApp.UI.Services;

public record TokenResponse(
    [property: JsonPropertyName("accessToken")] string AccessToken,
    [property: JsonPropertyName("refreshToken")] string RefreshToken,
    [property: JsonPropertyName("expiresIn")] int ExpiresIn,
    [property: JsonPropertyName("tokenType")] string TokenType
);

public class AuthService(IHttpClientFactory httpFactory, IJSRuntime js, IToastService toastService) : IAuthService
{
    private const string TokenKey = "auth_tokens";
    private readonly IToastService toastService = toastService;

    public async Task<bool> RegisterAsync(string email, string password, CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("Api");
        var resp = await http.PostAsJsonAsync("/api/auth/register", new { email, password }, ct);
        if (resp.IsSuccessStatusCode)
        {
            toastService.ShowSuccess("Registrering vellykket");
            return true;

        }
        else
        {
            toastService.ShowError("Registrering feilet: " + await resp.Content.ReadAsStringAsync(ct));
            return false;
        }

    }

    public async Task<bool> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("Api");
        var resp = await http.PostAsJsonAsync("/login?useCookies=false&useSessionCookies=false", new { email, password }, ct);
        if (!resp.IsSuccessStatusCode) return false;

        var tokens = await resp.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);
        if (tokens is null) return false;

        // Persist the raw JSON (camelCase due to attributes)
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, System.Text.Json.JsonSerializer.Serialize(tokens));
        return true;
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("ApiAuthed");
        _ = await http.PostAsync("/logout", null, ct);
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        if (string.IsNullOrWhiteSpace(json)) return null;
        var tokens = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(json);
        return tokens?.AccessToken;
    }
}
