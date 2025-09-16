using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.Owners;

public class OwnerService(IHttpClientFactory clients) : IOwnerService
{
    private readonly IHttpClientFactory _clients = clients;
    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<List<OwnerModel>> GetAllOwnersAsync()
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<OwnerModel>>("api/owner") ?? [];
    }

    public async Task<OwnerModel?> GetOwnerByIdAsync(int id)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<OwnerModel>($"api/owner/{id}");
    }

    public async Task AddOwnerAsync(OwnerModel owner)
    {
        var http = ApiAuthed();
        var result = await http.PostAsJsonAsync("api/owner", owner);
    }

    public async Task UpdateOwnerAsync(OwnerModel owner)
    {
        var http = ApiAuthed();
        var result = await http.PutAsJsonAsync($"api/owner/{owner.Id}", owner);
    }

    public async Task<bool> DeleteOwnerAsync(int id)
    {
        var http = ApiAuthed();
        var result = await http.DeleteAsync($"api/owner/{id}");
        return result.IsSuccessStatusCode;
    }


}
