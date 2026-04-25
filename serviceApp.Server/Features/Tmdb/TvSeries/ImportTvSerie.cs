using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class ImportTvSerie
{
    public record Command(int Tmdb) : ICommand<Response>;

    public record Response(int MediaItemId, bool AlreadyExisted = false, bool AlreadyInWatchlist = false);

    public class Handler : ICommandHandler<Command, Response>
    {
        private readonly TmdbClient _tmdb;
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;

        public Handler(TmdbClient tmdb, ApplicationDbContext db, IHttpContextAccessor httpContext)
        {
            _tmdb = tmdb;
            _db = db;
            _httpContext = httpContext;
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            try
            {
                var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                // 1️⃣ Finn eller opprett MediaItem
                var media = await _db.MediaItems
                    .FirstOrDefaultAsync(x => x.TmdbId == request.Tmdb && x.Type == MediaType.Series, ct);

                bool alreadyExisted = media != null;

                if (media == null)
                {
                    var tmdbShow = await _tmdb.GetTvDetailsAsync(request.Tmdb);
                    if (tmdbShow == null)
                        return Result.Fail<Response>("TV series not found in TMDB");

                    media = new MediaItem
                    {
                        TmdbId = request.Tmdb,
                        Type = MediaType.Series,
                        Title = tmdbShow.Name,
                        PosterPath = tmdbShow.PosterPath,
                        Seasons = tmdbShow.NumberOfSeasons,
                        Episodes = tmdbShow.NumberOfEpisodes
                    };

                    foreach (var s in tmdbShow.Seasons)
                    {
                        if (s.SeasonNumber == 0) continue;

                        media.SeasonsNav.Add(new Season
                        {
                            SeasonNumber = s.SeasonNumber,
                            Name = s.Name,
                            EpisodeCount = s.EpisodeCount,
                            AirDate = s.AirDateParsed,
                            PosterPath = s.PosterPath,
                            Episodes = new List<Episode>()
                        });
                    }

                    _db.MediaItems.Add(media);
                    await _db.SaveChangesAsync(ct);
                }

                // 2️⃣ Legg til i brukerens watchlist hvis ikke allerede der
                var inWatchlist = await _db.WatchlistItems.AnyAsync(
                    w => w.UserId == userId && w.MediaItemId == media.Id, ct);

                if (!inWatchlist)
                {
                    _db.WatchlistItems.Add(new WatchlistItem
                    {
                        UserId = userId,
                        MediaItemId = media.Id,
                        AddedAt = DateTime.UtcNow
                    });
                    await _db.SaveChangesAsync(ct);
                }

                return Result.Ok(new Response(media.Id, alreadyExisted, inWatchlist));
            }
            catch (Exception ex)
            {
                return Result.Fail<Response>($"Import failed: {ex.Message}");
            }
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("/api/tmdb/import/tv", async (ISender sender, Command command, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.Failure
                    ? Results.BadRequest(result.Error)
                    : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}
