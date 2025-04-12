using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.Owners;

public interface IOwnerService
{
    Task<List<OwnerModel>> GetAllOwnersAsync();
    Task<OwnerModel?> GetOwnerByIdAsync(int id);
    Task<OwnerModel> AddOwnerAsync(OwnerModel owner);
    Task<OwnerModel> UpdateOwnerAsync(OwnerModel owner);
    Task<bool> DeleteOwnerAsync(int id);
}
