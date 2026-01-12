namespace serviceApp.Server.Features.Images;

public class GetPrimaryImageSas : IEndpointDefinition
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet("/api/images/vehicle/{vehicleId:int}/primary-sas", async (
            int vehicleId,
            ApplicationDbContext db,
            AzureBlobImageService blobService,
            CancellationToken ct) =>
        {
            var img = await db.ImageFiles
                .Where(i => i.EntityType == ImageEntityType.Vehicle && i.EntityId == vehicleId)
                .OrderByDescending(i => i.IsPrimary).ThenByDescending(i => i.UploadedAt)
                .FirstOrDefaultAsync(ct);

            if (img is null) return Results.NoContent();

            // Parse blob path from full URL (supports nested folders)
            var uri = new Uri(img.Url);
            var path = uri.AbsolutePath.TrimStart('/');
            var slash = path.IndexOf('/');
            var blobName = slash >= 0 ? path[(slash + 1)..] : path;

            var sasUrl = await blobService.GetSasUrlAsync(blobName, TimeSpan.FromMinutes(10), ct);
            return Results.Ok(new { img.Id, Url = sasUrl });
        })
        .WithName("GetPrimaryVehicleImageSas");
    }
}
