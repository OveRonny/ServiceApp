namespace serviceApp.Server.Features.Images;

public static class ImageUploadEndpoints
{
    public static void MapImageUpload(this WebApplication app)
    {
        app.MapPost("/api/images/upload", async (
            HttpRequest request,
            AzureBlobImageService blobService,
            ApplicationDbContext db,
            CancellationToken ct) =>
        {
            if (!request.HasFormContentType)
                return Results.BadRequest("Form content type required");

            var form = await request.ReadFormAsync(ct);
            var file = form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return Results.BadRequest("No file uploaded");

            var allowed = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowed.Contains(file.ContentType))
                return Results.BadRequest("Only JPEG, PNG, GIF allowed");


            if (!form.TryGetValue("entityType", out var entityTypeStr) ||
               !form.TryGetValue("entityId", out var entityIdStr) ||
               !int.TryParse(entityIdStr, out var entityId) ||
               !int.TryParse(entityTypeStr, out var entityTypeInt) ||
               !Enum.IsDefined(typeof(ImageEntityType), entityTypeInt))
            {
                return Results.BadRequest("EntityType and EntityId are required and must be valid integers.");
            }

            var entityType = (ImageEntityType)entityTypeInt;

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            using var stream = file.OpenReadStream();
            var url = await blobService.UploadImageAsync(stream, fileName, file.ContentType, ct);

            // Save to DB
            var image = new ImageFile
            {
                Url = url,
                EntityType = entityType,
                EntityId = entityId,
                UploadedAt = DateTime.UtcNow
            };
            db.Add(image);
            await db.SaveChangesAsync(ct);

            return Results.Ok(new { image.Id, image.Url });
        })
        .Accepts<IFormFile>("multipart/form-data")
        .WithName("UploadImage");
    }
}
