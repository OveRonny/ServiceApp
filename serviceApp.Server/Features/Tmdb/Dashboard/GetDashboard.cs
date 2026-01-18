using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Dashboard;


public static class GetDashboard
{
    // Query
    public record Query : IQuery<DashboardDto>;

    // Root DTO
    public record DashboardDto(
        List<RecentWatchDto> RecentWatches,
        List<MostWatchedDto> MostWatched,
        DashboardStatsDto Stats
    );

    public record RecentWatchDto(
        int TmdbId,
        string Title,
        string? PosterPath,
        DateTime? WatchDate
    );

    public record MostWatchedDto(
        int TmdbId,
        string Title,
        string? PosterPath,
        int? TimesWatched
    );

    public record DashboardStatsDto(
        int? TotalWatches,
        int? TotalMinutesWatched
    );

    public class Handler : IQueryHandler<Query, DashboardDto>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;

        public Handler(ApplicationDbContext db, IHttpContextAccessor httpContext)
        {
            _db = db;
            _httpContext = httpContext;
        }

        public async Task<Result<DashboardDto>> Handle(
            Query request,
            CancellationToken ct)
        {
            var userId = _httpContext.HttpContext!
                .User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // 📅 Recently watched (siste visninger)
            var recentWatches = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.Progress >= 100)
                .OrderByDescending(w => w.WatchDate)
                .Take(10)
                .Select(w => new RecentWatchDto(
                    w.MediaItem.TmdbId,
                    w.MediaItem.Title,
                    w.MediaItem.PosterPath,
                    w.WatchDate
                ))
                .ToListAsync(ct);

            // 🔁 Most watched
            var mostWatched = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .GroupBy(w => w.MediaItem)
                .Select(g => new MostWatchedDto(
                    g.Key.TmdbId,
                    g.Key.Title,
                    g.Key.PosterPath,
                    g.Count()
                ))
                .OrderByDescending(x => x.TimesWatched)
                .Take(10)
                .ToListAsync(ct);

            // 📊 Stats
            var stats = new DashboardStatsDto(


                TotalWatches: await _db.WatchHistories
                    .CountAsync(w => w.UserId == userId, ct),

                TotalMinutesWatched: await _db.WatchHistories
                    .Where(w => w.UserId == userId)
                    .SumAsync(w => w.MediaItem.DurationMinutes, ct)
            );

            return Result.Ok(new DashboardDto(
                recentWatches,
                mostWatched,
                stats
            ));
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/dashboard", async (
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(), ct);
                return result.Failure
                    ? Results.BadRequest(result.Error)
                    : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }


}
