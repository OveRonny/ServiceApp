using serviceApp.Server.Features.Tmdb.TmdHelpers;
using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class GetMovieDetails
{
    public record Query(int TmdbId) : IQuery<MovieDetailsDto>;

    public record WatchHistoryDto(
      DateTime? WatchDate,
      double Progress,
      bool? Liked,
      int? Rating);

    public record MovieDetailsDto(
     int TmdbId,
     string Title,
     string? Overview,
     int? Runtime,
     List<string> Genres,
     string? PosterPath,
     string? MediaType,
     string? ReleaseDate,
     List<WatchHistoryDto> WatchHistory,
     bool IsImported
    )
    {
        public bool InWatchlist { get; init; }
        public DateTime? LastWatchedDate { get; init; }
        public double? LastProgress { get; init; }
    }



    public class Handler : IQueryHandler<Query, MovieDetailsDto>
    {
        private readonly ApplicationDbContext _db;
        private readonly TmdbClient _tmdb;
        private readonly IHttpContextAccessor httpContext;

        public Handler(ApplicationDbContext db, TmdbClient tmdb, IHttpContextAccessor _httpContext)
        {
            _db = db;
            _tmdb = tmdb;
            httpContext=_httpContext;
        }

        public async Task<Result<MovieDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
        {

            var userId = GetUserId();

            var watchData = await GetUserWatchData(request.TmdbId, userId, cancellationToken);

            var inWatchlist = await IsInWatchlist(request.TmdbId, userId, cancellationToken);

            var meta = await GetMovieMetadata(request.TmdbId, cancellationToken);
            if (meta == null)
                return Result.Fail<MovieDetailsDto>("Movie not found");

            // 1️⃣ Prøv å hente fra egen database
            var movie = await _db.MediaItems
                .Include(m => m.MediaItemGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.WatchHistories)
                .Include(m => m.WatchlistItems)
                .FirstOrDefaultAsync(m => m.TmdbId == request.TmdbId, cancellationToken);


            return Result.Ok(BuildDto(meta, watchData, inWatchlist));

        }

        private string GetUserId()
        {
            return httpContext.HttpContext!
                .User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        private async Task<List<WatchHistoryDto>> GetUserWatchData(int tmdbId, string userId, CancellationToken ct)
        {
            return await _db.WatchHistories
             .Where(w => w.UserId == userId && w.MediaItem.TmdbId == tmdbId)
             .OrderByDescending(w => w.WatchDate)
             .Select(w => new WatchHistoryDto(
                 w.WatchDate,
                 w.Progress,
                 w.Liked,
                 w.Rating
             ))
             .ToListAsync(ct);
        }

        private async Task<bool> IsInWatchlist(int tmdbId, string userId, CancellationToken ct)
        {
            return await _db.WatchlistItems
                .AnyAsync(w => w.MediaItem.TmdbId == tmdbId && w.UserId == userId, ct);
        }

        private async Task<MovieMeta?> GetMovieMetadata(int tmdbId, CancellationToken ct)
        {
            var movie = await _db.MediaItems
                .Include(m => m.MediaItemGenres)
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.TmdbId == tmdbId, ct);

            if (movie != null)
            {
                return new MovieMeta(
                    movie.TmdbId,
                    movie.Title,
                    movie.Overview,
                    movie.DurationMinutes,
                    movie.MediaItemGenres.Select(g => g.Genre.Name).ToList(),
                    movie.PosterPath,
                    "movie",
                    movie.ReleaseDate?.ToString("yyyy-MM-dd"),
                    IsImported: true
                );
            }

            var tmdbMovie = await _tmdb.GetMovieAsync(tmdbId);
            if (tmdbMovie == null)
                return null;

            return new MovieMeta(
                tmdbMovie.Id,
                tmdbMovie.Title,
                tmdbMovie.Overview,
                tmdbMovie.Runtime,
                tmdbMovie.Genres.Select(g => g.Name).ToList(),
                tmdbMovie.PosterPath,
                "movie",
                TmdbDateTimeHelper.ParseReleaseDate(tmdbMovie.ReleaseDate)?.ToString("yyyy-MM-dd"),
                IsImported: false
            );
        }

        private record MovieMeta(
            int TmdbId,
            string Title,
            string? Overview,
            int? Runtime,
            List<string> Genres,
            string? PosterPath,
            string MediaType,
            string? ReleaseDate,
            bool IsImported
        );

        private MovieDetailsDto BuildDto(MovieMeta meta, List<WatchHistoryDto> watchData, bool inWatchlist)
        {
            var last = watchData.FirstOrDefault();

            return new MovieDetailsDto(
                TmdbId: meta.TmdbId,
                Title: meta.Title,
                Overview: meta.Overview,
                Runtime: meta.Runtime,
                Genres: meta.Genres,
                PosterPath: meta.PosterPath,
                MediaType: meta.MediaType,
                ReleaseDate: meta.ReleaseDate,
                WatchHistory: watchData,
                IsImported: meta.IsImported
            )
            {
                InWatchlist = inWatchlist,
                LastWatchedDate = last?.WatchDate,
                LastProgress = last?.Progress
            };
        }



    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/movie/{tmdbId}", async (ISender sender, int tmdbId, CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(tmdbId), ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}

