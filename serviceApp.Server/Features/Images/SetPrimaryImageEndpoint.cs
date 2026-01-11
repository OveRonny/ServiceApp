namespace serviceApp.Server.Features.Images;

public class SetPrimaryImageEndpoint : IEndpointDefinition
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapPost("/api/images/{imageId:int}/set-primary", async (
            int imageId, ApplicationDbContext db, CancellationToken ct) =>
        {
            var img = await db.Set<ImageFile>().FirstOrDefaultAsync(i => i.Id == imageId, ct);
            if (img is null) return Results.NotFound();

            var siblings = db.Set<ImageFile>()
                .Where(i => i.EntityType == img.EntityType && i.EntityId == img.EntityId);

            await foreach (var s in siblings.AsAsyncEnumerable().WithCancellation(ct))
            {
                s.IsPrimary = s.Id == imageId;
            }

            await db.SaveChangesAsync(ct);
            return Results.Ok();
        })
        .WithName("SetPrimaryImage").RequireAuthorization();
    }
}
