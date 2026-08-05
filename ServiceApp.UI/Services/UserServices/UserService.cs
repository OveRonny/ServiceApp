
using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.UserServices;

public class UserService(IHttpClientFactory clients) : IUserService
{
    private readonly IHttpClientFactory _clients = clients;

    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<IEnumerable<string>> GetAllRolesAsync()
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<IEnumerable<string>>("api/roles")
             ?? Array.Empty<string>();
    }

    public async Task<(bool ok, string? error)> CreateUserAsync(CreateUserModel userModel)
    {
        var http = ApiAuthed();
        var user = new
        {
            userModel.Email,
            userModel.Password,
            userModel.PhoneNumber,
            userModel.Roles,
            userModel.CreateNewFamily
        };
        var response = await http.PostAsJsonAsync("api/user", user);
        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(error) ? "Noe gikk galt." : error);
    }

    public async Task<IReadOnlyList<AdminUserModel>> GetAdminUsersAsync() =>
        await ApiAuthed().GetFromJsonAsync<List<AdminUserModel>>("api/admin/users") ?? [];

    public async Task<(bool ok, string? error)> UpdateAdminUserAsync(string id, AdminUserEditModel model)
    {
        using var response = await ApiAuthed().PutAsJsonAsync($"api/admin/users/{id}", new
        {
            model.Email,
            model.PhoneNumber,
            model.EmailConfirmed,
            Roles = model.Roles.ToArray()
        });
        return await ResultAsync(response);
    }

    public async Task<(bool ok, string? error)> ResetAdminPasswordAsync(string id, string newPassword)
    {
        using var response = await ApiAuthed().PostAsJsonAsync($"api/admin/users/{id}/password", new { newPassword });
        return await ResultAsync(response);
    }

    public async Task<(bool ok, string? error)> SetAdminUserActiveAsync(string id, bool isActive)
    {
        using var response = await ApiAuthed().PostAsJsonAsync($"api/admin/users/{id}/active", new { isActive });
        return await ResultAsync(response);
    }

    private static async Task<(bool ok, string? error)> ResultAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return (true, null);
        var error = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(error) ? "Noe gikk galt." : error.Trim('"'));
    }
}
