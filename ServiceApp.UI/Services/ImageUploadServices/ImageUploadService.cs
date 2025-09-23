using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ImageUploadServices;

public class ImageUploadService(IHttpClientFactory httpFactory) : IImageUploadService
{
    public async Task<string?> UploadImageAsync(IBrowserFile file, CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("ApiAuthed");
        using var content = new MultipartFormDataContent();
        var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10 MB limit
        content.Add(new StreamContent(stream)
        {
            Headers = { ContentType = new MediaTypeHeaderValue(file.ContentType) }
        }, "file", file.Name);

        var resp = await http.PostAsync("/api/images/upload", content, ct);
        if (!resp.IsSuccessStatusCode) return null;
        var result = await resp.Content.ReadFromJsonAsync<UploadResult>(cancellationToken: ct);
        return result?.Url;
    }

    private record UploadResult(string Url);


    public async Task<List<string>?> GetImageUrlsAsync(CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("ApiAuthed");
        return await http.GetFromJsonAsync<List<string>>("/api/images/list", cancellationToken: ct);
    }

    public async Task<List<int>?> GetServiceRecordImageIdsAsync(int serviceRecordId, CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("ApiAuthed");
        return await http.GetFromJsonAsync<List<int>>($"/api/service-record/{serviceRecordId}/images", ct);
    }
}
