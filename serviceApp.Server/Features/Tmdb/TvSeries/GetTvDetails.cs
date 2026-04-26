namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class GetTvDetails
{
    public record Query(int TmdbId) : IQuery<TmdbTvDetailsDto>;

    public record TmdbTvDetailsDto(
      int TmdbId,
      string Name,
      string OriginalName,
      string? Overview,
      string? FirstAirDate,
      string? LastAirDate,
      int NumberOfSeasons,
      int NumberOfEpisodes,
      string? PosterPath,
      string? BackdropPath,
      List<TmdbSeasonDto> Seasons,
      List<TmdbCreatorDto> CreatedBy,
      List<string> Genres,
      double VoteAverage
  );

    public class Handler : IQueryHandler<Query, TmdbTvDetailsDto>
    {
        private readonly TmdbClient _tmdb;
        private readonly ApplicationDbContext _db;

        public Handler(TmdbClient tmdb, ApplicationDbContext db)
        {
            _tmdb = tmdb;
            _db = db;
        }

        public async Task<Result<TmdbTvDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Prøv databasen først
            var media = await _db.MediaItems
                .AsNoTracking()
                .Include(m => m.SeasonsNav)
                .Include(m => m.MediaItemGenres)
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.TmdbId == request.TmdbId && m.Type == MediaType.Series, cancellationToken);

            if (media != null)
            {
                var dbSeasons = media.SeasonsNav
                    .OrderBy(s => s.SeasonNumber)
                    .Select(s => new TmdbSeasonDto
                    {
                        SeasonNumber = s.SeasonNumber,
                        Name = s.Name,
                        Overview = s.Overview,
                        PosterPath = s.PosterPath,
                        EpisodeCount = s.EpisodeCount ?? 0,
                        AirDate = s.AirDate?.ToString("yyyy-MM-dd"),
                        VoteAverage = s.VoteAverage
                    }).ToList();

                var dbGenres = media.MediaItemGenres
                    .Select(mg => mg.Genre.Name)
                    .ToList();

                return Result.Ok(new TmdbTvDetailsDto(
                    media.TmdbId,
                    media.Title,
                    media.Title,
                    media.Overview,
                    media.ReleaseDate?.ToString("yyyy-MM-dd"),
                    null,
                    media.Seasons ?? 0,
                    media.Episodes ?? 0,
                    media.PosterPath,
                    null,
                    dbSeasons,
                    new List<TmdbCreatorDto>(),
                    dbGenres,
                    0
                ));
            }

            // Fallback: hent fra TMDB hvis ikke i databasen
            var tvSeries = await _tmdb.GetTvDetailsAsync(request.TmdbId);
            if (tvSeries == null)
                return Result.Fail<TmdbTvDetailsDto>("TV series not found");

            var seasons = tvSeries.Seasons.Select(s => new TmdbSeasonDto
            {
                Id = s.Id,
                SeasonNumber = s.SeasonNumber,
                Name = s.Name,
                Overview = s.Overview,
                PosterPath = s.PosterPath,
                EpisodeCount = s.EpisodeCount,
                AirDate = s.AirDate,
                VoteAverage = s.VoteAverage
            }).ToList();

            var creator = tvSeries.CreatedBy.Select(c => new TmdbCreatorDto
            {
                Id = c.Id,
                Name = c.Name,
                ProfilePath = c.ProfilePath
            }).ToList();

            return Result.Ok(new TmdbTvDetailsDto(
                tvSeries.Id,
                tvSeries.Name,
                tvSeries.OriginalName,
                tvSeries.Overview,
                tvSeries.FirstAirDate,
                tvSeries.LastAirDate,
                tvSeries.NumberOfSeasons,
                tvSeries.NumberOfEpisodes,
                tvSeries.PosterPath,
                tvSeries.BackdropPath,
                seasons,
                creator,
                tvSeries.Genres.Select(g => g.Name).ToList(),
                tvSeries.VoteAverage
            ));
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/tv/{tmdbId}", async (ISender sender, int tmdbId, CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(tmdbId), ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}