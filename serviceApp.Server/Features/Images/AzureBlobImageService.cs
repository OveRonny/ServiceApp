using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace serviceApp.Server.Features.Images;

public class AzureBlobImageService
{
    private readonly BlobContainerClient _container;

    public AzureBlobImageService(IConfiguration config)
    {
        var connStr = config["AzureStorage:ConnectionString"] ?? throw new InvalidOperationException("AzureStorage:ConnectionString missing");
        var containerName = config["AzureStorage:ContainerName"] ?? "images";
        _container = new BlobContainerClient(connStr, containerName);
        _container.CreateIfNotExists(PublicAccessType.None);
    }

    public async Task<string> UploadImageAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var blob = _container.GetBlobClient(fileName);
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);
        return blob.Uri.ToString();
    }

    public async Task<Stream?> GetImageStreamAsync(string blobName, CancellationToken ct = default)
    {
        var blob = _container.GetBlobClient(blobName);
        if (!await blob.ExistsAsync(ct)) return null;
        return await blob.OpenReadAsync(cancellationToken: ct);
    }

    public string GetSasUrl(string blobName, TimeSpan validFor)
    {
        var blobClient = _container.GetBlobClient(blobName);
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _container.Name,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(validFor)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
}
