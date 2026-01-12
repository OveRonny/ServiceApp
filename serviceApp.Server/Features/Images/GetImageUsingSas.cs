namespace serviceApp.Server.Features.Images;

public class GetImageUsingSas : IEndpointDefinition
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet("/api/images/{id}/sas", async (int id, ApplicationDbContext db, AzureBlobImageService blobService, CancellationToken ct) =>
        {
            var image = await db.ImageFiles.FindAsync([id], ct);
            if (image is null) return Results.NotFound();

            // Parse blob path correctly even for nested folders
            var uri = new Uri(image.Url);
            var path = uri.AbsolutePath.TrimStart('/');
            var slash = path.IndexOf('/');
            var blobName = slash >= 0 ? path[(slash + 1)..] : path;

            var sasUrl = await blobService.GetSasUrlAsync(blobName, TimeSpan.FromMinutes(10), ct);

            return Results.Ok(new { Url = sasUrl });
        }).RequireAuthorization();
    }
}
