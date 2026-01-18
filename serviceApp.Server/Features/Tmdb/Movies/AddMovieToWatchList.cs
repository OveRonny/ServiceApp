using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class AddMovieToWatchList
{
    public record Command(int TmdbId) : ICommand;

    public class Handler : ICommandHandler<Command>
    {
        private readonly ApplicationDbContext _db;
        private readonly TmdbClient _tmdb;
        private readonly IHttpContextAccessor _http;

        public Handler(
            ApplicationDbContext db,
            TmdbClient tmdb,
            IHttpContextAccessor http)
        {
            _db = db;
            _tmdb = tmdb;
            _http = http;
        }

        public async Task<Result> Handle(
            Command request,
            CancellationToken ct)
        {
            var userId = _http.HttpContext!
                .User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // 1️⃣ Finn MediaItem (eller importer)
            var media = await _db.MediaItems
                .FirstOrDefaultAsync(m => m.TmdbId == request.TmdbId, ct);

            if (media == null)
            {
                var movie = await _tmdb.GetMovieAsync(request.TmdbId);
                if (movie == null)
                    return Result.Fail("Fant ikke filmen i TMDb");

                media = new MediaItem
                {
                    TmdbId = movie.Id,
                    Title = movie.Title,
                    Overview = movie.Overview,
                    DurationMinutes = movie.Runtime,
                    PosterPath = movie.PosterPath,
                    Type = MediaType.Movie
                };

                foreach (var g in movie.Genres)
                {
                    var genre = await _db.Genres
                        .FirstOrDefaultAsync(x => x.Name == g.Name, ct)
                        ?? new Genre { Name = g.Name };

                    media.MediaItemGenres.Add(
                        new MediaItemGenre { Genre = genre });
                }

                _db.MediaItems.Add(media);
            }

            // 2️⃣ Sjekk om allerede i watchlist
            var exists = await _db.WatchlistItems.AnyAsync(w =>
                w.UserId == userId &&
                w.MediaItem.TmdbId == request.TmdbId,
                ct);

            if (exists)
                return Result.Fail("Filmen er allerede i skal-se-listen");

            // 3️⃣ Legg til i watchlist
            _db.WatchlistItems.Add(new WatchlistItem
            {
                UserId = userId,
                MediaItem = media,
                AddedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(ct);

            return Result.Ok();
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("/api/tmdb/movies/watchlist", async (ISender sender, Command command, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.Success
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            })
            .RequireAuthorization();
        }
    }
}


