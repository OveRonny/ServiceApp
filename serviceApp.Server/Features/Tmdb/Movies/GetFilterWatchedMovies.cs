using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class GetFilterWatchedMovies
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
            var query = _db.MediaItems
                .AsNoTracking()
                .Where(m => m.Type == MediaType.Movie &&
                            m.WatchHistories.Any(w => w.UserId == userId && w.Progress >= 100 && w.SeasonId == null && w.EpisodeId == null));

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(m =>
                    EF.Functions.Like(m.Title, $"%{search}%"));
            }

            // 3️⃣ Genre filter
            if (!string.IsNullOrWhiteSpace(request.Genre))
            {
                query = query.Where(m =>
                    m.MediaItemGenres.Any(g =>
                        g.Genre.Name == request.Genre));
            }

            // 4️⃣ Total count (før paging!)
            var total = await query.CountAsync(ct);

            // 5️⃣ Page items
            var mediaItems = await query
                .Include(m => m.MediaItemGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.WatchHistories)
                .OrderByDescending(m =>
                    m.WatchHistories
                        .Where(w => w.WatchDate != null)
                        .Max(w => w.WatchDate))
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            var tasks = mediaItems
              .Select(m =>
              {
                  var lastWatch = m.WatchHistories
                      .Where(w => w.UserId == userId && w.Progress >= 100 && w.SeasonId == null && w.EpisodeId == null)
                      .OrderByDescending(w => w.WatchDate)
                      .FirstOrDefault();

                  return new MovieFullDto
                  {
                      MediaItemId = m.Id,
                      TmdbId = m.TmdbId,
                      Title = m.Title,
                      PosterPath = m.PosterPath,
                      Overview = m.Overview,
                      Runtime = m.DurationMinutes,
                      WatchedDate = lastWatch?.WatchDate,
                      Genres = m.MediaItemGenres
                          .Select(g => g.Genre.Name)
                          .ToList()
                  };
              });

            var movies = tasks.ToArray();

            var result = new PagedResult<MovieFullDto>
            {
                Items = movies,
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return Result.Ok(result);
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/movies/watched/filter", async (ISender sender,
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
