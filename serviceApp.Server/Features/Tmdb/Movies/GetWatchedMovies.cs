using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class GetWatchedMovies
{
    public record Query(int Page = 1, int PageSize = 50) : IQuery<List<MovieFullDto>>;

    public record Response(List<MovieFullDto> Movies);

    public class Handler(ApplicationDbContext db, TmdbClient tmdb, IHttpContextAccessor httpContext) : IQueryHandler<Query, List<MovieFullDto>>
    {
        private readonly ApplicationDbContext db = db;
        private readonly TmdbClient tmdb = tmdb;
        private readonly IHttpContextAccessor httpContext = httpContext;

        public async Task<Result<List<MovieFullDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Hent MediaItems brukeren har sett
            var watchedMedia = await db.MediaItems
                .Include(m => m.WatchHistories)
                .Where(m => m.WatchHistories.Any(w => w.UserId == userId && w.Progress >= 100))
                .OrderByDescending(m => m.WatchHistories.FirstOrDefault()!.WatchDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            if (watchedMedia == null || watchedMedia.Count == 0)
                return Result.Ok(new List<MovieFullDto>());

            // Hent TMDb-data parallelt
            var tmdbTasks = watchedMedia.Select(async m =>
            {

                var tmdbMovie = await tmdb.GetMovieAsync(m.TmdbId);
                if (tmdbMovie == null) return null;


                return new MovieFullDto
                {
                    MediaItemId = m.Id,
                    TmdbId = m.TmdbId,
                    Title = tmdbMovie.Title,
                    PosterPath = tmdbMovie.PosterUrl,
                    Overview = tmdbMovie.Overview,
                    Runtime = tmdbMovie.Runtime,
                    WatchedDate = m.WatchHistories.FirstOrDefault()?.WatchDate,
                    Genres = tmdbMovie.Genres.Select(g => g.Name).ToList()
                };



            });

            var moviesNullable = await Task.WhenAll(tmdbTasks);

            var movies = moviesNullable
            .Where(m => m != null)    // fjern null
            .Select(m => m!)           // konverter MovieFullDto? -> MovieFullDto
            .ToList();

            if (movies.Count > 0)
                return Result.Ok(movies);

            return Result.Fail<List<MovieFullDto>>("Failed to retrieve watched movies.");

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
