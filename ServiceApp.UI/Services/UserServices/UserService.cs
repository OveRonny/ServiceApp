
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

    public async Task CreateUserAsync(CreateUserModel userModel)
    {
        var http = ApiAuthed();
        var user = new
        {
            userModel.UserName,
            userModel.Email,
            userModel.Password,
            userModel.PhoneNumber,
            userModel.Roles
        };
        await http.PostAsJsonAsync("api/user", user);
    }
}
