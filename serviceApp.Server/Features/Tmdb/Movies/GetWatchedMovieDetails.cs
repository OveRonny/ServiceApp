using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class GetWatchedMovieDetails
{
    public record Query(int MediaItemId) : IQuery<WatchedMovieDetailsDto>;

    public record WatchHistoryDto(
      DateTime WatchDate,
      double Progress,
      bool? Liked,
      int? Rating
  );

    public record WatchedMovieDetailsDto(
    int TmdbId,
    string Title,
    string? Overview,
    int? Runtime,
    List<string> Genres,
    string? PosterPath,
    List<WatchHistoryDto> WatchHistory
    );


    public class Handler : IQueryHandler<Query, WatchedMovieDetailsDto>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;
        private readonly TmdbClient _tmdb;

        public Handler(ApplicationDbContext db, IHttpContextAccessor httpContext, TmdbClient tmdb)
        {
            _db = db;
            _httpContext = httpContext;
            _tmdb = tmdb;
        }

        public async Task<Result<WatchedMovieDetailsDto>> Handle(Query request, CancellationToken ct)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Hent MediaItem + WatchHistory
            var media = await _db.MediaItems
                .Include(m => m.WatchHistories)
                .Include(m => m.MediaItemGenres)
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == request.MediaItemId, ct);

            if (media == null)
                return Result.Fail<WatchedMovieDetailsDto>("Movie not found");

            // Hent TMDb-data
            var tmdbMovie = await _tmdb.GetMovieAsync(media.TmdbId);

            var dto = new WatchedMovieDetailsDto(
                TmdbId: media.TmdbId,
                Title: tmdbMovie?.Title ?? media.Title,
                Overview: tmdbMovie?.Overview,
                Runtime: tmdbMovie?.Runtime,
                Genres: media.MediaItemGenres.Select(g => g.Genre.Name).ToList(),
                PosterPath: tmdbMovie?.PosterUrl,
                WatchHistory: media.WatchHistories
                    .OrderByDescending(w => w.WatchDate)
                    .Select(w => new WatchHistoryDto(
                        WatchDate: w.WatchDate!.Value,
                        Progress: w.Progress,
                        Liked: w.Liked,
                        Rating: w.Rating
                    ))
                    .ToList()
            );

            return Result.Ok(dto);
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/movies/watched/details/{mediaItemId:int}", async (ISender sender, int mediaItemId, CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(mediaItemId), ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}
