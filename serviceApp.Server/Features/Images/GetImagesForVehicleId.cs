namespace serviceApp.Server.Features.Images;

public class GetImagesForVehicleId : IEndpointDefinition
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet("/api/images/{entityType:int}/{entityId:int}", async (
            int entityType,
            int entityId,
            ApplicationDbContext db,
            AzureBlobImageService blobService,
            CancellationToken ct) =>
        {
            var images = await db.ImageFiles
                .Where(i => (int)i.EntityType == entityType && i.EntityId == entityId)
                .OrderByDescending(i => i.IsPrimary)
                .ThenByDescending(i => i.UploadedAt)
                .ToListAsync(ct);

            if (images.Count == 0) return Results.NoContent();

            var expiry = TimeSpan.FromMinutes(10);

            var resultTasks = images.Select(async img =>
            {
                var uri = new Uri(img.Url);
                var path = uri.AbsolutePath.TrimStart('/');
                var slash = path.IndexOf('/');
                var blobPath = slash >= 0 ? path[(slash + 1)..] : path; // preserve folders

                var sasUrl = await blobService.GetSasUrlAsync(blobPath, expiry, ct);

                return new ImageFile
                {
                    Id = img.Id,
                    Url = sasUrl,
                    IsPrimary = img.IsPrimary,
                    UploadedAt = img.UploadedAt
                };
            });

            var result = await Task.WhenAll(resultTasks);
            return Results.Ok(result);
        });
    }
}