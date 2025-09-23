namespace serviceApp.Server.Features.Images;

public class GetImageUsingSas : IEndpointDefinition
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet("/api/images/{id}/sas", async (int id, ApplicationDbContext db, AzureBlobImageService blobService) =>
        {
            var image = await db.ImageFiles.FindAsync(id);
            if (image == null) return Results.NotFound();

            var blobName = new Uri(image.Url).Segments.Last();
            var sasUrl = blobService.GetSasUrl(blobName, TimeSpan.FromMinutes(10)); // 10 minutes validity

            return Results.Ok(new { Url = sasUrl });
        }).RequireAuthorization();
    }
}
