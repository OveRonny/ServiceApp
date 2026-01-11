using Microsoft.AspNetCore.Components.Forms;
using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.ImageUploadServices;

public interface IImageUploadService
{
    Task<ImageModel?> UploadImageAsync(ImageEntityType entityType, int entityId, IBrowserFile file, CancellationToken ct = default);
    Task<ImageModel?> UploadImageAsync(IBrowserFile file, CancellationToken ct = default);

    Task<ImageModel?> GetPrimaryVehicleImageAsync(int vehicleId, CancellationToken ct = default);
    Task<ImageModel?> GetPrimaryVehicleImageSasAsync(int vehicleId, CancellationToken ct = default); // NEW

    Task<System.Collections.Generic.List<ImageModel>> GetImagesAsync(ImageEntityType entityType, int entityId, CancellationToken ct = default);
    Task SetPrimaryAsync(int imageId, CancellationToken ct = default);
    Task DeleteAsync(int imageId, CancellationToken ct = default);

    void SetUploadContext(ImageEntityType entityType, int entityId);
}

