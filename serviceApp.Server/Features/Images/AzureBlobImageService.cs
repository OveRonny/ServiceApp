using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace serviceApp.Server.Features.Images;

public class AzureBlobImageService
{
    private readonly BlobContainerClient _container;
    private readonly TokenCredential _credential;
    private readonly string? _accountName;
    private readonly string? _connectionString;

    public AzureBlobImageService(IConfiguration config)
    {
        _credential = new ChainedTokenCredential(
            new ManagedIdentityCredential(),
            new AzureCliCredential(),
            new VisualStudioCredential(),
            new AzureDeveloperCliCredential()
        );

        var containerName = config["AzureStorage:ContainerName"] ?? "images";
        _connectionString = config["AzureStorage:ConnectionString"];

        if (!string.IsNullOrWhiteSpace(_connectionString))
        {
            _container = new BlobContainerClient(_connectionString, containerName);
            _accountName = null;
        }
        else
        {
            _accountName = config["AzureStorage:AccountName"] ?? throw new InvalidOperationException("AzureStorage:AccountName missing");
            var containerUri = new Uri($"https://{_accountName}.blob.core.windows.net/{containerName}");
            _container = new BlobContainerClient(containerUri, _credential);
        }

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

    // Generate a user delegation SAS (works with Managed Identity).
    public async Task<string> GetSasUrlAsync(string blobName, TimeSpan validFor, CancellationToken ct = default)
    {
        var blobClient = _container.GetBlobClient(blobName);
        var expiresOn = DateTimeOffset.UtcNow.Add(validFor);

        if (!string.IsNullOrWhiteSpace(_connectionString))
        {
            // Local/dev: key-based SAS using connection string (shared key)
            var sas = new BlobSasBuilder
            {
                BlobContainerName = _container.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = expiresOn
            };
            sas.SetPermissions(BlobSasPermissions.Read);

            // Generate SAS directly from the blob client (shared key from connection string)
            var sasUri = blobClient.GenerateSasUri(sas);
            return sasUri.ToString();
        }

        // Azure: user delegation SAS using Managed Identity
        if (string.IsNullOrWhiteSpace(_accountName))
            throw new InvalidOperationException("User delegation SAS requires AzureStorage:AccountName and Managed Identity.");

        var serviceClientMi = new BlobServiceClient(new Uri($"https://{_accountName}.blob.core.windows.net/"), _credential);
        var key = await serviceClientMi.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, expiresOn, ct);

        var udSas = new BlobSasBuilder
        {
            BlobContainerName = _container.Name,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = expiresOn
        };
        udSas.SetPermissions(BlobSasPermissions.Read);

        var udSasUri = blobClient.GenerateUserDelegationSasUri(udSas, key.Value);
        return udSasUri.ToString();
    }

    public async Task<bool> DeleteAsync(string blobName, CancellationToken ct = default)
    {
        var client = _container.GetBlobClient(blobName);
        var response = await client.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: ct);
        return response.Value;
    }
}
