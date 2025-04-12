using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.Owners;

public class OwnerService(HttpClient http) : IOwnerService
{
    private readonly HttpClient http = http;

    public async Task<List<OwnerModel>> GetAllOwnersAsync()
    {
        var response = await http.GetFromJsonAsync<List<OwnerModel>>("api/owner");
        return response ?? new List<OwnerModel>();
    }

    public async Task<OwnerModel?> GetOwnerByIdAsync(int id)
    {

        var response = await http.GetFromJsonAsync<OwnerModel>($"api/owner/{id}");
        if (response == null)
        {
            throw new Exception($"Owner with ID {id} not found.");
        }
        return response;

    }

    public async Task<OwnerModel> AddOwnerAsync(OwnerModel owner)
    {
        var result = await http.PostAsJsonAsync("api/owner", owner);
        if (result.IsSuccessStatusCode)
        {
            var createdOwner = await result.Content.ReadFromJsonAsync<OwnerModel>();
            return createdOwner ?? new OwnerModel();
        }
        else
        {
            throw new Exception("Failed to add owner");
        }
    }
    public async Task<OwnerModel> UpdateOwnerAsync(OwnerModel owner)
    {
        var result = await http.PutAsJsonAsync($"api/owner/{owner.Id}", owner);
        if (result.IsSuccessStatusCode)
        {
            var updatedOwner = await result.Content.ReadFromJsonAsync<OwnerModel>();
            return updatedOwner ?? new OwnerModel();
        }
        else
        {
            throw new Exception($"Failed to update owner with ID {owner.Id}. Status Code: {result.StatusCode}");
        }

    }

    public async Task<bool> DeleteOwnerAsync(int id)
    {
        var result = await http.DeleteAsync($"api/owner/{id}");
        if (result.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            throw new Exception($"Failed to delete owner with ID {id}. Status Code: {result.StatusCode}");
        }
    }


}
