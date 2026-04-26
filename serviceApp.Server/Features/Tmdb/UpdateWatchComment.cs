using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb;

public static class UpdateWatchComment
{
    public record Command(int TmdbId, string? MediaType, string? Comment) : ICommand;

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
                "movie"         => (Entities.MediaType?)Entities.MediaType.Movie,
                "series" or "tv" => (Entities.MediaType?)Entities.MediaType.Series,
                _                => (Entities.MediaType?)null
            };

            var query = _db.MediaItems.Where(m => m.TmdbId == request.TmdbId);
            if (mediaType.HasValue)
                query = query.Where(m => m.Type == mediaType.Value);

            var mediaItemId = await query.Select(m => m.Id).FirstOrDefaultAsync(ct);

            if (mediaItemId == 0)
                return Result.Fail("Fant ikke media");

            var lastWatch = await _db.WatchHistories
                .Where(w => w.UserId == userId && w.MediaItemId == mediaItemId && w.EpisodeId == null && w.SeasonId == null)
                .OrderByDescending(w => w.WatchDate)
                .FirstOrDefaultAsync(ct);

            if (lastWatch == null)
                return Result.Fail("Ingen seehistorikk funnet");

            lastWatch.Comment = request.Comment;
            await _db.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPatch("/api/tmdb/watchhistory/comment", async (ISender sender, [Microsoft.AspNetCore.Mvc.FromBody] Command command, CancellationToken ct) =>
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
