using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.StatensVegvesenServices;

public interface IStatensVegvesenService
{
    Task<StatensVegvesenModel> GetVehicleInfo(string registrationNumber);
}
