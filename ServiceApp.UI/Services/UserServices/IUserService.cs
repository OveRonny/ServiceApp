using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.UserServices;

public interface IUserService
{
    Task<(bool ok, string? error)> CreateUserAsync(CreateUserModel userModel);

    Task<IEnumerable<string>> GetAllRolesAsync();
}
