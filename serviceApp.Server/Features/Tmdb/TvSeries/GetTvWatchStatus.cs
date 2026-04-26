using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class GetTvWatchStatus
{
    public record Query(int TmdbId) : IQuery<TvWatchStatusDto>;

    public class Handler : IQueryHandler<Query, TvWatchStatusDto>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;

        public Handler(ApplicationDbContext db, IHttpContextAccessor httpContext)
        {
            _db = db;
            _httpContext = httpContext;
        }

        public async Task<Result<TvWatchStatusDto>> Handle(Query request, CancellationToken ct)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var media = await _db.MediaItems
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.TmdbId == request.TmdbId && m.Type == MediaType.Series, ct);

            if (media == null)
                return Result.Ok(new TvWatchStatusDto());

            var isInWatchlist = await _db.WatchlistItems
                .AnyAsync(w => w.UserId == userId && w.MediaItemId == media.Id, ct);

            var streamingService = await _db.WatchlistItems
                .Where(w => w.UserId == userId && w.MediaItemId == media.Id)
                .Select(w => (int?)w.StreamingService)
                .FirstOrDefaultAsync(ct);

            var lastComment = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.MediaItemId == media.Id && w.SeasonId == null && w.EpisodeId == null)
                .OrderByDescending(w => w.WatchDate)
                .Select(w => w.Comment)
                .FirstOrDefaultAsync(ct);

            var watchedSeasonIds = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.MediaItemId == media.Id && w.SeasonId != null && w.EpisodeId == null)
                .Select(w => w.SeasonId!.Value)
                .Distinct()
                .ToListAsync(ct);

            var watchedSeasonNumbers = await _db.Seasons
                .AsNoTracking()
                .Where(s => watchedSeasonIds.Contains(s.Id))
                .Select(s => s.SeasonNumber)
                .ToListAsync(ct);

            var watchedEpisodeIds = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.MediaItemId == media.Id && w.EpisodeId != null)
                .Select(w => w.EpisodeId!.Value)
                .Distinct()
                .ToListAsync(ct);

            // Count watched episodes per season
            var watchedEpisodeCountBySeason = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.MediaItemId == media.Id && w.EpisodeId != null)
                .Join(_db.Episodes, wh => wh.EpisodeId!.Value, ep => ep.Id, (wh, ep) => ep.SeasonId)
                .Join(_db.Seasons, seasonId => seasonId, s => s.Id, (_, s) => s.SeasonNumber)
                .GroupBy(sn => sn)
                .Select(g => new { SeasonNumber = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            return Result.Ok(new TvWatchStatusDto
            {
                MediaItemId = media.Id,
                IsInWatchlist = isInWatchlist,
                WatchedSeasonNumbers = watchedSeasonNumbers,
                WatchedEpisodeIds = watchedEpisodeIds,
                WatchedEpisodeCountBySeason = watchedEpisodeCountBySeason.ToDictionary(x => x.SeasonNumber, x => x.Count),
                StreamingService = streamingService,
                Comment = lastComment
            });
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/tv/{tmdbId}/watch-status", async (ISender sender, int tmdbId, CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(tmdbId), ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}
