using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace ServiceApp.UI.Services;

public class JwtAuthenticationStateProvider(IAuthService auth, IHttpClientFactory httpFactory) : AuthenticationStateProvider
{
    private static ClaimsPrincipal Anonymous => new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await auth.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(Anonymous);

        // Try JWT first
        var parts = token.Split('.');
        if (parts.Length >= 2)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        // Fallback: opaque token -> ask server for user info/roles
        try
        {
            var http = httpFactory.CreateClient("ApiAuthed");
            var me = await http.GetFromJsonAsync<UserInfo>("api/auth/me");
            if (me is not null)
            {
                var claims = new List<Claim>();
                if (!string.IsNullOrWhiteSpace(me.Email)) claims.Add(new Claim(ClaimTypes.Name, me.Email));
                if (!string.IsNullOrWhiteSpace(me.Id)) claims.Add(new Claim(ClaimTypes.NameIdentifier, me.Id));
                if (me.Roles is not null)
                    claims.AddRange(me.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

                var identity = new ClaimsIdentity(claims, "opaque");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch
        {
            // ignore and treat as anonymous
        }

        return new AuthenticationState(Anonymous);
    }

    public void NotifyUserAuthentication(string token)
    {
        var parts = token.Split('.');
        var identity = parts.Length >= 2
            ? new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")
            : new ClaimsIdentity(authenticationType: "opaque"); // roles will be populated on next state fetch
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public void NotifyUserLogout() =>
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2) yield break;

        var payload = parts[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? new();

        var roleKeys = new[] { "role", "roles", ClaimTypes.Role };

        foreach (var kvp in dict)
        {
            if (roleKeys.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase))
            {
                if (kvp.Value is JsonElement el)
                {
                    if (el.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var r in el.EnumerateArray())
                        {
                            var v = r.GetString();
                            if (!string.IsNullOrWhiteSpace(v))
                                yield return new Claim(ClaimTypes.Role, v);
                        }
                        continue;
                    }
                    if (el.ValueKind == JsonValueKind.String)
                    {
                        var v = el.GetString();
                        if (!string.IsNullOrWhiteSpace(v))
                            yield return new Claim(ClaimTypes.Role, v);
                        continue;
                    }
                }
                var v2 = kvp.Value?.ToString();
                if (!string.IsNullOrWhiteSpace(v2))
                    yield return new Claim(ClaimTypes.Role, v2);
                continue;
            }

            yield return new Claim(kvp.Key, kvp.Value?.ToString() ?? "");
        }
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='));
    }

    private sealed record UserInfo(string Id, string? Email, string[] Roles);
}
