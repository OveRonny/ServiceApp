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

        public Handler(TmdbClient tmdb) => _tmdb = tmdb;

        public async Task<Result<TmdbTvDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var tvSeries = await _tmdb.GetTvDetailsAsync(request.TmdbId); // fetch full details from TMDb
            if (tvSeries == null)
                return Result.Fail<TmdbTvDetailsDto>("TV series not found");

            var Seasons = tvSeries.Seasons.Select(s => new TmdbSeasonDto
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

            var dto = new TmdbTvDetailsDto(
                 tvSeries.Id,                               // TmdbId
                 tvSeries.Name,                             // Name
                 tvSeries.OriginalName,                     // OriginalName
                 tvSeries.Overview,                         // Overview
                 tvSeries.FirstAirDate,                     // FirstAirDate
                 tvSeries.LastAirDate,                      // LastAirDate
                 tvSeries.NumberOfSeasons,                  // NumberOfSeasons
                 tvSeries.NumberOfEpisodes,                 // NumberOfEpisodes
                 tvSeries.PosterPath,                       // PosterPath
                 tvSeries.BackdropPath,
                 Seasons,
                 creator,
                 tvSeries.Genres.Select(g => g.Name).ToList(),
                 tvSeries.VoteAverage
            );





            return Result.Ok(dto);
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
