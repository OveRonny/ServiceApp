namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class ImportTvSerie
{
    public record Command(int Tmdb) : ICommand<Response>;

    public record Response(int MediaItemId);

    public class Handler : ICommandHandler<Command, Response>
    {
        private readonly TmdbClient _tmdb;
        private readonly ApplicationDbContext _db;

        public Handler(TmdbClient tmdb, ApplicationDbContext db)
        {
            _tmdb = tmdb;
            _db = db;
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            // 1️⃣ Sjekk om serien allerede finnes globalt
            var media = await _db.MediaItems
                .Include(x => x.Seasons)
                .FirstOrDefaultAsync(x => x.TmdbId == request.Tmdb && x.Type == MediaType.Series, ct);

            // 2️⃣ Hvis ikke, hent fra TMDB
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
                    if (s.SeasonNumber == 0) continue; // hopp over specials

                    // Sjekk om sesongen allerede finnes
                    if (media.SeasonsNav.Any(x => x.SeasonNumber == s.SeasonNumber))
                        continue;

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
            }

            await _db.SaveChangesAsync(ct);

            return Result.Ok(new Response(media.Id));
        }
    }
}

