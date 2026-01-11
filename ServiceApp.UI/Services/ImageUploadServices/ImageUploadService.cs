using Microsoft.AspNetCore.Components.Forms;
using ServiceApp.UI.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ImageUploadServices;

public class ImageUploadService(IHttpClientFactory httpFactory) : IImageUploadService
{

    private ImageEntityType? _ctxType;
    private int? _ctxId;

    public void SetUploadContext(ImageEntityType entityType, int entityId)
    {
        _ctxType = entityType;
        _ctxId = entityId;
    }


    public async Task<ImageModel?> UploadImageAsync(ImageEntityType entityType, int entityId, IBrowserFile file, CancellationToken ct = default)
    {
        const long maxBytes = 10 * 1024 * 1024;
        if (file is null) throw new ArgumentNullException(nameof(file));
        if (file.Size > maxBytes)
            throw new InvalidOperationException($"File is too large ({file.Size:n0} bytes). Max {maxBytes:n0} bytes.");

        var http = httpFactory.CreateClient("ApiAuthed");
        using var content = new MultipartFormDataContent();

        using var stream = file.OpenReadStream(maxAllowedSize: maxBytes, cancellationToken: ct);
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        content.Add(fileContent, "file", file.Name);
        content.Add(new StringContent(((int)entityType).ToString()), "entityType");
        content.Add(new StringContent(entityId.ToString()), "entityId");
        // ask server to make this the primary
        content.Add(new StringContent("true"), "makePrimary");

        using var resp = await http.PostAsync("/api/images/upload", content, ct);
        if (!resp.IsSuccessStatusCode)
        {
            var details = await resp.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Upload failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. {details}");
        }

        return await resp.Content.ReadFromJsonAsync<ImageModel>(cancellationToken: ct);
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

    public async Task<ImageModel?> GetPrimaryVehicleImageAsync(int vehicleId, CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("ApiAuthed");
        using var resp = await http.GetAsync($"/api/images/vehicle/{vehicleId}/primary", ct);

        if (resp.StatusCode == HttpStatusCode.NoContent || resp.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!resp.IsSuccessStatusCode)
            return null;

        // If server returned 200 with empty body (defensive)
        if (resp.Content.Headers.ContentLength is long len && len == 0)
            return null;

        return await resp.Content.ReadFromJsonAsync<ImageModel?>(cancellationToken: ct);
    }

    public async Task<ImageModel?> GetPrimaryVehicleImageSasAsync(int vehicleId, CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("ApiAuthed");
        using var resp = await http.GetAsync($"/api/images/vehicle/{vehicleId}/primary-sas", ct);

        if (resp.StatusCode == HttpStatusCode.NoContent || resp.StatusCode == HttpStatusCode.NotFound)
            return null;
        if (!resp.IsSuccessStatusCode)
            return null;

        // { id, url } returned by server
        return await resp.Content.ReadFromJsonAsync<ImageModel?>(cancellationToken: ct);
    }

    public async Task<List<ImageModel>> GetImagesAsync(ImageEntityType entityType, int entityId, CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("ApiAuthed");

        var url = entityType == ImageEntityType.Vehicle
            ? $"/api/images/{(int)ImageEntityType.Vehicle}/{entityId}" // "/api/images/0/{id}"
            : $"/api/images/{(int)entityType}/{entityId}";

        if (http.BaseAddress is null)
            throw new InvalidOperationException("ApiAuthed HttpClient BaseAddress is not configured.");

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
        linked.CancelAfter(TimeSpan.FromSeconds(20)); // avoid indefinite hangs

        try
        {
            using var resp = await http.GetAsync(url, linked.Token);

            if (resp.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.NotFound)
                return new List<ImageModel>();

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(linked.Token);
                throw new HttpRequestException($"GET {url} failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. {body}");
            }

            if (resp.Content.Headers.ContentLength is long len && len == 0)
                return new List<ImageModel>();

            return await resp.Content.ReadFromJsonAsync<List<ImageModel>>(cancellationToken: ct) ?? new List<ImageModel>();
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException($"GET {url} timed out after 20s.");
        }
    }

    public async Task SetPrimaryAsync(int imageId, CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("ApiAuthed");
        var resp = await http.PostAsync($"/api/images/{imageId}/set-primary", null, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int imageId, CancellationToken ct = default)
    {
        var http = httpFactory.CreateClient("ApiAuthed");
        var resp = await http.DeleteAsync($"/api/images/{imageId}", ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<ImageModel?> UploadImageAsync(IBrowserFile file, CancellationToken ct = default)
    {
        if (_ctxType is null || _ctxId is null)
            throw new InvalidOperationException("Upload context not set. Call SetUploadContext(entityType, entityId) before UploadImageAsync(file).");

        return await UploadImageAsync(_ctxType.Value, _ctxId.Value, file, ct);
    }
}
