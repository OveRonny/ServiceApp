namespace serviceApp.Server.Features.Images;

public class DeleteImage : IEndpointDefinition
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapDelete("/api/images/{imageId:int}", async (
            int imageId,
            ApplicationDbContext db,
            AzureBlobImageService blobService,
            CancellationToken ct) =>
        {
            var img = await db.ImageFiles.FirstOrDefaultAsync(i => i.Id == imageId, ct);
            if (img is null) return Results.NotFound();

            // Extract blob path from the stored URL, preserving virtual folders
            // Expected URL format: https://{account}.blob.core.windows.net/{container}/{blobPath...}
            var uri = new Uri(img.Url);
            var path = uri.AbsolutePath.TrimStart('/'); // "{container}/{blobPath...}"
            var slash = path.IndexOf('/');
            var blobPath = slash >= 0 ? path[(slash + 1)..] : path;

            var deleted = await blobService.DeleteAsync(blobPath, ct);
            if (!deleted)
            {
                // Optionally log; continue to remove DB row to avoid orphaned metadata
                // return Results.Problem($"Failed to delete blob '{container}/{blobPath}'.");
            }

            db.ImageFiles.Remove(img);
            await db.SaveChangesAsync(ct);

            return Results.NoContent();
        }).RequireAuthorization();
    }
}
