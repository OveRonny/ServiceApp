using Microsoft.AspNetCore.Components.Forms;

namespace ServiceApp.UI.Services.ImageUploadServices;
public interface IImageUploadService
{
    Task<List<string>?> GetImageUrlsAsync(CancellationToken ct = default);
    Task<string?> UploadImageAsync(IBrowserFile file, CancellationToken ct = default);
}