using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class GetUnwatchedTvSeries
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

            var query = _db.WatchlistItems
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.MediaItem.Type == MediaType.Series)
                .Where(w => w.MediaItem.SeasonsNav
                    .SelectMany(s => s.Episodes)
                    .Any(e => !w.MediaItem.WatchHistories
                        .Any(wh => wh.UserId == userId && wh.EpisodeId == e.Id)));

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(w => EF.Functions.Like(w.MediaItem.Title, $"%{request.Search.Trim()}%"));

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(w => w.MediaItem.Title)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(w => new TvSeriesListDto
                {
                    MediaItemId = w.MediaItem.Id,
                    TmdbId = w.MediaItem.TmdbId,
                    Title = w.MediaItem.Title,
                    PosterPath = w.MediaItem.PosterPath,
                    TotalSeasons = w.MediaItem.Seasons,
                    AddedToWatchlistAt = w.AddedAt
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
            app.MapGet("/api/tmdb/tv/unwatched", async (
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
