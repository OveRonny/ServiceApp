using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class GetWatchedMovies
{
    public record Query(int Page = 1, int PageSize = 50) : IQuery<List<MovieFullDto>>;

    public record Response(List<MovieFullDto> Movies);

    public class Handler(ApplicationDbContext db, IHttpContextAccessor httpContext) : IQueryHandler<Query, List<MovieFullDto>>
    {
        private readonly ApplicationDbContext db = db;
        private readonly IHttpContextAccessor httpContext = httpContext;

        public async Task<Result<List<MovieFullDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Hent MediaItems brukeren har sett
            var watchedMedia = await db.MediaItems
                .Include(m => m.WatchHistories)
                .Include(m => m.MediaItemGenres)
                    .ThenInclude(mg => mg.Genre)
                .Where(m => m.Type == MediaType.Movie &&
                            m.WatchHistories.Any(w => w.UserId == userId && w.Progress >= 100 && w.SeasonId == null && w.EpisodeId == null))
                .OrderByDescending(m => m.WatchHistories
                    .Where(w => w.UserId == userId)
                    .Max(w => w.WatchDate))
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            if (watchedMedia == null || watchedMedia.Count == 0)
                return Result.Ok(new List<MovieFullDto>());

            var movies = watchedMedia.Select(m =>
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
                    Genres = m.MediaItemGenres.Select(g => g.Genre.Name).ToList()
                };
            }).ToList();

            return Result.Ok(movies);

        }


    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/movies/watched", async (ISender sender, CancellationToken ct, int page = 1, int pageSize = 50) =>
            {
                var result = await sender.Send(new Query(page, pageSize), ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }



}
