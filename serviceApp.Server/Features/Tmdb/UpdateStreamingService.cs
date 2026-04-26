using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb;

public static class UpdateStreamingService
{
    public record Command(int TmdbId, int? StreamingService, string? MediaType) : ICommand;

    public class Handler : ICommandHandler<Command>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;

        public Handler(ApplicationDbContext db, IHttpContextAccessor httpContext)
        {
            _db = db;
            _httpContext = httpContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken ct)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var mediaType = request.MediaType?.ToLower() switch
            {
                "movie" => (MediaType?)Entities.MediaType.Movie,
                "series" or "tv" => (MediaType?)Entities.MediaType.Series,
                _ => (MediaType?)null
            };

            var query = _db.MediaItems.Where(m => m.TmdbId == request.TmdbId);
            if (mediaType.HasValue)
                query = query.Where(m => m.Type == mediaType.Value);

            var mediaItemId = await query
                .Select(m => m.Id)
                .FirstOrDefaultAsync(ct);

            if (mediaItemId == 0)
                return Result.Fail("Fant ikke media");

            var item = await _db.WatchlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MediaItemId == mediaItemId, ct);

            if (item == null)
            {
                item = new WatchlistItem
                {
                    UserId = userId,
                    MediaItemId = mediaItemId,
                    AddedAt = DateTime.UtcNow,
                    StreamingService = request.StreamingService.HasValue
                        ? (Entities.StreamingService?)request.StreamingService.Value
                        : null
                };
                _db.WatchlistItems.Add(item);
            }
            else
            {
                item.StreamingService = request.StreamingService.HasValue
                    ? (Entities.StreamingService?)request.StreamingService.Value
                    : null;
            }

            await _db.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPatch("/api/tmdb/watchlist/streaming", async (ISender sender, [Microsoft.AspNetCore.Mvc.FromBody] Command command, CancellationToken ct) =>
            {
                if (command is null)
                    return Results.BadRequest("Ugyldig forespørsel");
                var result = await sender.Send(command, ct);
                return result.Success ? Results.Ok() : Results.BadRequest(result.Error);
            })
            .RequireAuthorization();
        }
    }
}
