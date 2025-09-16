using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.SupplierServices;

public class SupplierService(IHttpClientFactory clients) : ISupplierService
{
    private readonly IHttpClientFactory _clients = clients;

    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<List<SupplierModel>> GetSuppliersAsync()
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<SupplierModel>>("api/supplier") ?? [];
    }

    public async Task<SupplierModel?> GetSupplierByIdAsync(int id)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<SupplierModel>($"api/supplier/{id}");
    }

    public async Task CreateSupplierAsync(SupplierModel supplier)
    {
        var http = ApiAuthed();
        await http.PostAsJsonAsync("api/supplier", supplier);
    }

    public async Task UpdateSupplierAsync(SupplierModel supplier)
    {
        var http = ApiAuthed();
        await http.PutAsJsonAsync($"api/supplier/{supplier.Id}", supplier);
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        var http = ApiAuthed();
        var resp = await http.DeleteAsync($"api/supplier/{id}");
        return resp.IsSuccessStatusCode;
    }
}
