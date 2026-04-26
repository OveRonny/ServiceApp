using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class GetUnWatchedMovies
{
    public record Query(
     int Page,
     int PageSize,
     string? Search,
     string? Genre
     ) : IQuery<PagedResult<MovieFullDto>>;




    public class Handler : IQueryHandler<Query, PagedResult<MovieFullDto>>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;


        public Handler(
            ApplicationDbContext db,
            IHttpContextAccessor httpContext)

        {
            _db = db;
            _httpContext = httpContext;
        }

        public async Task<Result<PagedResult<MovieFullDto>>> Handle(
            Query request,
            CancellationToken ct)
        {
            var userId = _httpContext.HttpContext!
                .User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // 1️⃣ Base query
            var query = _db.WatchlistItems
                .AsNoTracking()
                .Where(w => w.UserId == userId &&
                            w.MediaItem.Type == MediaType.Movie &&
                            !w.MediaItem.WatchHistories.Any(wh => wh.UserId == userId && wh.Progress >= 100 && wh.SeasonId == null && wh.EpisodeId == null));

            // Search filter
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(w => EF.Functions.Like(w.MediaItem.Title, $"%{search}%"));
            }

            // Genre filter
            if (!string.IsNullOrWhiteSpace(request.Genre))
            {
                query = query.Where(w => w.MediaItem.MediaItemGenres
                    .Any(g => g.Genre.Name == request.Genre));
            }

            // 4️⃣ Total count (før paging!)
            var total = await query.CountAsync(ct);

            // 5️⃣ Page items
            var mediaItems = await query
              .OrderBy(w => w.MediaItem.Title)
              .Skip((request.Page - 1) * request.PageSize)
              .Take(request.PageSize)
              .Select(w => new MovieFullDto
              {
                  MediaItemId = w.MediaItem.Id,
                  TmdbId = w.MediaItem.TmdbId,
                  Title = w.MediaItem.Title,
                  Overview = w.MediaItem.Overview,
                  Runtime = w.MediaItem.DurationMinutes,
                  PosterPath = w.MediaItem.PosterPath,
                  Genres = w.MediaItem.MediaItemGenres
                      .Select(g => g.Genre.Name)
                      .ToList()
              })
              .ToListAsync(ct);

            return Result.Ok(new PagedResult<MovieFullDto>
            {
                Items = mediaItems,
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
            app.MapGet("/api/tmdb/movies/un-watched/filter", async (ISender sender,
                int page,
                int pageSize,
                string? search,
                string? genre,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new Query(page, pageSize, search, genre), ct);
                return result.Failure
                    ? Results.BadRequest(result.Error)
                    : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }

    }

}

