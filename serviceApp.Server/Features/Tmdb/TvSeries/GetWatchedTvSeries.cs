using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class GetWatchedTvSeries
{
    public record Query(int Page, int PageSize, string? Search) : IQuery<PagedResult<TvSeriesListDto>>;

    public class Handler : IQueryHandler<Query, PagedResult<TvSeriesListDto>>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;

        public Handler(ApplicationDbContext db, IHttpContextAccessor httpContext)
        {
            _db = db;
            _httpContext = httpContext;
        }

        public async Task<Result<PagedResult<TvSeriesListDto>>> Handle(Query request, CancellationToken ct)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var query = _db.MediaItems
                .AsNoTracking()
                .Where(m => m.Type == MediaType.Series
                         && m.WatchHistories.Any(w => w.UserId == userId));

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(m => EF.Functions.Like(m.Title, $"%{request.Search.Trim()}%"));

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(m => m.WatchHistories
                    .Where(w => w.UserId == userId)
                    .Max(w => (DateTime?)w.WatchDate))
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new TvSeriesListDto
                {
                    MediaItemId = m.Id,
                    TmdbId = m.TmdbId,
                    Title = m.Title,
                    PosterPath = m.PosterPath,
                    TotalSeasons = m.Seasons,
                    WatchedSeasons = m.WatchHistories
                        .Where(w => w.UserId == userId && w.SeasonId != null)
                        .Select(w => w.SeasonId)
                        .Distinct()
                        .Count(),
                    LastWatchedDate = m.WatchHistories
                        .Where(w => w.UserId == userId)
                        .Max(w => (DateTime?)w.WatchDate)
                })
                .ToListAsync(ct);

            return Result.Ok(new PagedResult<TvSeriesListDto>
            {
                Items = items,
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize
            });
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/tv/watched", async (
                ISender sender,
                int page = 1,
                int pageSize = 24,
                string? search = null,
                CancellationToken ct = default) =>
            {
                var result = await sender.Send(new Query(page, pageSize, search), ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}
