using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.UserServices;

public interface IUserService
{
    Task CreateUserAsync(CreateUserModel userModel);

    Task<IEnumerable<string>> GetAllRolesAsync();
}
