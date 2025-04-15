using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.SupplierServices;

public class SupplierService(HttpClient http) : ISupplierService
{
    private readonly HttpClient http = http;

    public async Task<List<SupplierModel>> GetSuppliersAsync()
    {
        var result = await http.GetFromJsonAsync<List<SupplierModel>>("api/supplier");
        if (result == null)
        {
            throw new Exception("Error retrieving suppliers");
        }
        return result;
    }

    public async Task<SupplierModel> GetSupplierByIdAsync(int id)
    {
        var result = await http.GetFromJsonAsync<SupplierModel>($"api/supplier/{id}");
        if (result == null)
        {
            throw new Exception($"Supplier with id {id} not found");
        }
        return result;
    }

    public async Task<SupplierModel> CreateSupplierAsync(SupplierModel supplier)
    {
        var result = await http.PostAsJsonAsync("api/supplier", supplier);
        if (result.IsSuccessStatusCode)
        {
            var newSupplier = await result.Content.ReadFromJsonAsync<SupplierModel>();
            if (newSupplier != null)
            {
                return newSupplier;
            }
            else
            {
                throw new Exception("Error deserializing supplier");
            }
        }
        else
        {
            throw new Exception("Error creating supplier");
        }
    }

    public async Task<SupplierModel> UpdateSupplierAsync(SupplierModel supplier)
    {
        var result = await http.PutAsJsonAsync($"api/supplier/{supplier.Id}", supplier);
        if (result.IsSuccessStatusCode)
        {
            var updatedSupplier = await result.Content.ReadFromJsonAsync<SupplierModel>();
            if (updatedSupplier != null)
            {
                return updatedSupplier;
            }
            else
            {
                throw new Exception("Error deserializing supplier");
            }
        }
        else
        {
            throw new Exception("Error updating supplier");
        }
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        var result = await http.DeleteAsync($"api/supplier/{id}");
        if (result.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            throw new Exception("Error deleting supplier");
        }
    }
}
