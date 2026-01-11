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

            var blobName = new Uri(img.Url).Segments.Last();
            var sasUrl = blobService.GetSasUrl(blobName, TimeSpan.FromMinutes(10));
            return Results.Ok(new { img.Id, Url = sasUrl });
        })
        .WithName("GetPrimaryVehicleImageSas");
    }
}
