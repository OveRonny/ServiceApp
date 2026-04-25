using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class MarkEpisodeAsWatched
{
    public record Command(int EpisodeId, bool Watched, int MediaItemId) : ICommand<Response>;

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

            if (request.Watched)
            {
                var exists = await _db.WatchHistories.AnyAsync(
                    w => w.UserId == userId && w.MediaItemId == request.MediaItemId && w.EpisodeId == request.EpisodeId, ct);

                if (!exists)
                {
                    var episode = await _db.Episodes
                        .Include(e => e.Season)
                        .FirstOrDefaultAsync(e => e.Id == request.EpisodeId, ct);

                    _db.WatchHistories.Add(new WatchHistory
                    {
                        MediaItemId = request.MediaItemId,
                        EpisodeId = request.EpisodeId,
                        SeasonId = episode?.SeasonId,
                        UserId = userId,
                        Progress = 100,
                        WatchDate = DateTime.UtcNow
                    });
                    await _db.SaveChangesAsync(ct);
                }
            }
            else
            {
                var entry = await _db.WatchHistories.FirstOrDefaultAsync(
                    w => w.UserId == userId && w.MediaItemId == request.MediaItemId && w.EpisodeId == request.EpisodeId, ct);

                if (entry != null)
                {
                    _db.WatchHistories.Remove(entry);
                    await _db.SaveChangesAsync(ct);
                }
            }

            return Result.Ok(new Response(true));
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("/api/tmdb/tv/mark-episode", async (ISender sender, Command command, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}
