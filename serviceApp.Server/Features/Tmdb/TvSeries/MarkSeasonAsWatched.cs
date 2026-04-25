using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class MarkSeasonAsWatched
{
    public record Command(int TmdbId, int SeasonNumber, bool Watched) : ICommand<Response>;

    public record Response(bool Success);

    public class Handler : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;

        public Handler(ApplicationDbContext db, IHttpContextAccessor httpContext)
        {
            _db = db;
            _httpContext = httpContext;
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var media = await _db.MediaItems
                .Include(m => m.SeasonsNav)
                .FirstOrDefaultAsync(m => m.TmdbId == request.TmdbId && m.Type == MediaType.Series, ct);

            if (media == null)
                return Result.Fail<Response>("TV series not found. Import it first.");

            var season = media.SeasonsNav.FirstOrDefault(s => s.SeasonNumber == request.SeasonNumber);
            if (season == null)
                return Result.Fail<Response>($"Season {request.SeasonNumber} not found.");

            if (request.Watched)
            {
                var exists = await _db.WatchHistories.AnyAsync(
                    w => w.UserId == userId && w.MediaItemId == media.Id && w.SeasonId == season.Id, ct);

                if (!exists)
                {
                    _db.WatchHistories.Add(new WatchHistory
                    {
                        MediaItemId = media.Id,
                        SeasonId = season.Id,
                        UserId = userId,
                        Progress = 100,
                        WatchDate = DateTime.UtcNow
                    });
                }
            }
            else
            {
                var entry = await _db.WatchHistories.FirstOrDefaultAsync(
                    w => w.UserId == userId && w.MediaItemId == media.Id && w.SeasonId == season.Id, ct);

                if (entry != null)
                    _db.WatchHistories.Remove(entry);
            }

            await _db.SaveChangesAsync(ct);
            return Result.Ok(new Response(true));
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("/api/tmdb/tv/mark-season", async (ISender sender, Command command, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}
