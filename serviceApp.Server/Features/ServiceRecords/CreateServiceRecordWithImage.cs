using serviceApp.Server.Features.Images;
using System.Text.Json;

namespace serviceApp.Server.Features.ServiceRecords;

public class CreateServiceRecordWithImage : IEndpointDefinition
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapPost("/api/service-record/with-image", async (
            HttpRequest request,
            ISender sender,
            ApplicationDbContext db,
            AzureBlobImageService blobService,
            CancellationToken ct) =>
        {
            if (!request.HasFormContentType)
                return Results.BadRequest("Form content type required");

            var form = await request.ReadFormAsync(ct);

            // Parse ServiceRecord command from JSON
            if (!form.TryGetValue("command", out var commandJson))
                return Results.BadRequest("Missing service record data.");

            var command = JsonSerializer.Deserialize<CreateServiceRecord.Command>(commandJson!);
            if (command == null)
                return Results.BadRequest("Invalid service record data.");

            var result = await sender.Send(command, ct);
            if (result.Failure)
                return Results.BadRequest(result.Error);

            var serviceRecordId = result.Value!.Id;

            // Handle image file
            var file = form.Files.FirstOrDefault();
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                using var stream = file.OpenReadStream();
                var url = await blobService.UploadImageAsync(stream, fileName, file.ContentType, ct);

                var image = new ImageFile
                {
                    Url = url,
                    EntityType = ImageEntityType.ServiceRecord,
                    EntityId = serviceRecordId,
                    UploadedAt = DateTime.UtcNow
                };
                db.Add(image);
                await db.SaveChangesAsync(ct);
            }

            return Results.Ok(result.Value);
        }).RequireAuthorization();
    }
}
