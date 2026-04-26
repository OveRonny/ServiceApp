using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class MarkMovieAsWatched
{
    public record Command(
        int TmdbId,
        bool MarkAsWatched = false,
        DateTime? Date = null,
        bool? Liked = null,
        int? Rating = null,
        string? Comment = null
    ) : ICommand<Response>;

    public record Response(bool Success);

    public class Handler : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;
        private readonly TmdbClient _tmdb;

        public Handler(ApplicationDbContext db, IHttpContextAccessor http, TmdbClient tmdb)
        {
            _db = db;
            _httpContext = http;
            _tmdb = tmdb;
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // 1️⃣ Finn eller importer media
            var media = await GetOrImportMedia(request.TmdbId, cancellationToken);
            if (media == null)
                return Result.Fail<Response>("Movie not found");

            if (request.MarkAsWatched)
            {
                var newWatch = CreateWatchHistory(media.Id, userId, request.Date);

                newWatch.Liked = request.Liked;
                newWatch.Rating = request.Rating;
                newWatch.Comment = request.Comment;

                _db.WatchHistories.Add(newWatch);
            }
            else if (request.Liked.HasValue || request.Rating.HasValue || request.Comment != null)
            {
                await UpdateLastWatchHistoryAsync(media.Id, userId, request.Liked, request.Rating, request.Comment);
            }

            // 4️⃣ Fjern fra Watchlist
            await RemoveFromWatchlist(media.Id, userId, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(new Response(true));
        }

        private async Task<MediaItem?> GetOrImportMedia(int tmdbId, CancellationToken ct)
        {
            var media = await _db.MediaItems.FirstOrDefaultAsync(m => m.TmdbId == tmdbId && m.Type == MediaType.Movie, ct);
            if (media != null) return media;

            var movie = await _tmdb.GetMovieAsync(tmdbId);
            if (movie == null) return null;

            media = new MediaItem
            {
                TmdbId = movie.Id,
                Title = movie.Title,
                Overview = movie.Overview,
                DurationMinutes = movie.Runtime,
                PosterPath = movie.PosterPath,
                Type = MediaType.Movie
            };
            _db.MediaItems.Add(media);
            return media;
        }

        private WatchHistory CreateWatchHistory(
          int mediaItemId,
          string userId,
          DateTime? date)
        {
            return new WatchHistory
            {
                MediaItemId = mediaItemId,
                UserId = userId,
                Progress = 100,
                WatchDate = date ?? DateTime.UtcNow
            };
        }

        private async Task RemoveFromWatchlist(int mediaItemId, string userId, CancellationToken ct)
        {
            var item = await _db.WatchlistItems.FirstOrDefaultAsync(
                w => w.MediaItemId == mediaItemId && w.UserId == userId, ct);

            if (item != null)
                _db.WatchlistItems.Remove(item);
        }

        public async Task<bool> UpdateLastWatchHistoryAsync(int mediaItemId, string userId, bool? liked = null, int? rating = null, string? comment = null)
        {
            var lastWatch = await _db.WatchHistories
                .Where(w => w.MediaItemId == mediaItemId && w.UserId == userId)
                .OrderByDescending(w => w.WatchDate)
                .FirstOrDefaultAsync();

            if (lastWatch == null) return false;

            if (liked.HasValue) lastWatch.Liked = liked.Value;
            if (rating.HasValue) lastWatch.Rating = rating;
            if (comment != null) lastWatch.Comment = comment;

            await _db.SaveChangesAsync();
            return true;
        }


    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("/api/movies/markaswatched", async (ISender sender, Command command, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }




}
