namespace serviceApp.Server.Features.Images;

public class ServiceRecordImagesEndpoint : IEndpointDefinition
{
    public void MapEndpoints(WebApplication app)
    {
        // List image IDs for a ServiceRecord
        app.MapGet("/api/service-record/{serviceRecordId}/images", async (int serviceRecordId, ApplicationDbContext db) =>
        {
            var imageIds = await db.ImageFiles
                .Where(img => img.EntityType == ImageEntityType.ServiceRecord && img.EntityId == serviceRecordId)
                .Select(img => img.Id)
                .ToListAsync();

            return Results.Ok(imageIds);
        }).RequireAuthorization();

        // Proxy endpoint to stream the image by ID
        app.MapGet("/api/images/{id:int}", async (int id, ApplicationDbContext db, AzureBlobImageService blobService) =>
        {
            var image = await db.ImageFiles.FindAsync(id);
            if (image == null) return Results.NotFound();

            var blobName = new Uri(image.Url).Segments.Last();
            var stream = await blobService.GetImageStreamAsync(blobName);
            if (stream == null) return Results.NotFound();

            // Optionally, store ContentType in DB; here we default to image/jpeg
            return Results.File(stream, "image/jpeg");
        }).RequireAuthorization();
    }
}
