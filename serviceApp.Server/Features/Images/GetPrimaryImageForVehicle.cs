namespace serviceApp.Server.Features.Images;

public static class GetPrimaryImageForVehicle
{
    public static void GetImageForVehicle(this WebApplication app)
    {
        app.MapGet("/api/images/vehicle/{vehicleId:int}/primary", async (
        int vehicleId, ApplicationDbContext db, CancellationToken ct) =>
        {
            var primary = await db.Set<ImageFile>()
                .Where(i => i.EntityType == ImageEntityType.Vehicle && i.EntityId == vehicleId)
                .OrderByDescending(i => i.IsPrimary).ThenByDescending(i => i.UploadedAt)
                .Select(i => new { i.Id, i.Url })
                .FirstOrDefaultAsync(ct);

            return primary is null
                ? Results.NoContent()
                : Results.Ok(primary);
        })
    .WithName("GetPrimaryVehicleImage");
    }
}
