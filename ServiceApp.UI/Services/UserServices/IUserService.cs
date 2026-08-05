using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.UserServices;

public interface IUserService
{
    Task<(bool ok, string? error)> CreateUserAsync(CreateUserModel userModel);

    Task<IEnumerable<string>> GetAllRolesAsync();
    Task<IReadOnlyList<AdminUserModel>> GetAdminUsersAsync();
    Task<(bool ok, string? error)> UpdateAdminUserAsync(string id, AdminUserEditModel model);
    Task<(bool ok, string? error)> ResetAdminPasswordAsync(string id, string newPassword);
    Task<(bool ok, string? error)> SetAdminUserActiveAsync(string id, bool isActive);

}
