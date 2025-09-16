using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.Owners;

public interface IOwnerService
{
    Task<List<OwnerModel>> GetAllOwnersAsync();
    Task<OwnerModel?> GetOwnerByIdAsync(int id);
    Task AddOwnerAsync(OwnerModel owner);
    Task UpdateOwnerAsync(OwnerModel owner);
    Task<bool> DeleteOwnerAsync(int id);
}
