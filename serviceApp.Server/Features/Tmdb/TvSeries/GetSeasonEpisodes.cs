using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class GetSeasonEpisodes
{
    public record Query(int TmdbId, int SeasonNumber) : IQuery<SeasonEpisodesDto>;

    public class Handler : IQueryHandler<Query, SeasonEpisodesDto>
    {
        private readonly ApplicationDbContext _db;
        private readonly TmdbClient _tmdb;
        private readonly IHttpContextAccessor _httpContext;

        public Handler(ApplicationDbContext db, TmdbClient tmdb, IHttpContextAccessor httpContext)
        {
            _db = db;
            _tmdb = tmdb;
            _httpContext = httpContext;
        }

        public async Task<Result<SeasonEpisodesDto>> Handle(Query request, CancellationToken ct)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var media = await _db.MediaItems
                .Include(m => m.SeasonsNav)
                    .ThenInclude(s => s.Episodes)
                .FirstOrDefaultAsync(m => m.TmdbId == request.TmdbId && m.Type == MediaType.Series, ct);

            // Series not imported — fetch directly from TMDB without watch status
            if (media == null)
            {
                var tmdbSeasonOnly = await _tmdb.GetSeasonAsync(request.TmdbId, request.SeasonNumber);
                if (tmdbSeasonOnly == null)
                    return Result.Fail<SeasonEpisodesDto>("Fant ikke sesongen på TMDB.");

                return Result.Ok(new SeasonEpisodesDto
                {
                    SeasonNumber = request.SeasonNumber,
                    Episodes = tmdbSeasonOnly.Episodes.Select(e => new EpisodeWithStatusDto
                    {
                        Id = 0,
                        EpisodeNumber = e.EpisodeNumber,
                        Name = e.Name,
                        Overview = e.Overview,
                        VoteAverage = e.VoteAverage,
                        IsWatched = false
                    }).ToList()
                });
            }

            var season = media.SeasonsNav.FirstOrDefault(s => s.SeasonNumber == request.SeasonNumber);
            if (season == null)
                return Result.Fail<SeasonEpisodesDto>($"Sesong {request.SeasonNumber} ikke funnet.");

            // Lazy-load episodes from TMDB if not stored yet
            if (!season.Episodes.Any())
            {
                var tmdbSeason = await _tmdb.GetSeasonAsync(request.TmdbId, request.SeasonNumber);
                if (tmdbSeason != null)
                {
                    foreach (var ep in tmdbSeason.Episodes)
                    {
                        season.Episodes.Add(new Episode
                        {
                            EpisodeNumber = ep.EpisodeNumber,
                            Name = ep.Name,
                            Overview = ep.Overview,
                            AirDate = string.IsNullOrEmpty(ep.AirDate) ? null : DateTime.Parse(ep.AirDate),
                            VoteAverage = ep.VoteAverage,
                            SeasonId = season.Id
                        });
                    }
                    await _db.SaveChangesAsync(ct);
                }
            }

            // Get watch history (episodeId → watchDate) for this user + media
            var watchHistoryByEpisode = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.MediaItemId == media.Id && w.EpisodeId != null)
                .Select(w => new { EpisodeId = w.EpisodeId!.Value, w.WatchDate })
                .ToListAsync(ct);

            var watchedMap = watchHistoryByEpisode
                .GroupBy(w => w.EpisodeId)
                .ToDictionary(g => g.Key, g => g.Max(w => w.WatchDate));

            var episodeDtos = season.Episodes
                .OrderBy(e => e.EpisodeNumber)
                .Select(e => new EpisodeWithStatusDto
                {
                    Id = e.Id,
                    EpisodeNumber = e.EpisodeNumber,
                    Name = e.Name,
                    Overview = e.Overview,
                    VoteAverage = e.VoteAverage,
                    IsWatched = watchedMap.ContainsKey(e.Id),
                    WatchedDate = watchedMap.GetValueOrDefault(e.Id)
                })
                .ToList();

            return Result.Ok(new SeasonEpisodesDto
            {
                SeasonNumber = request.SeasonNumber,
                Episodes = episodeDtos
            });
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/tv/{tmdbId}/season/{seasonNumber}/episodes",
                async (ISender sender, int tmdbId, int seasonNumber, CancellationToken ct) =>
                {
                    var result = await sender.Send(new Query(tmdbId, seasonNumber), ct);
                    return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
                })
            .RequireAuthorization();
        }
    }
}
