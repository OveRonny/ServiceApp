using serviceApp.Server.Features.Tmdb.TmdHelpers;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class GetMovieDetailsFromTmdb
{
    public record Query(int TmdbId) : IQuery<MovieDetailsDto>;
    public record MovieDetailsDto(
        int TmdbId,
        string Title,
        string? Overview,
        int? Runtime,
        List<string> Genres,
        string? PosterPath,
        string? MediaType,
        string? ReleaseDate
    );
    public class Handler(TmdbClient tmdb) : IQueryHandler<Query, MovieDetailsDto>
    {
        private readonly TmdbClient tmdb = tmdb;
        public async Task<Result<MovieDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var movie = await tmdb.GetMovieAsync(request.TmdbId);
            if (movie == null)
                return Result.Fail<MovieDetailsDto>("Movie not found in TMDb");

            var dto = new MovieDetailsDto(
              TmdbId: movie.Id,
              Title: movie.Title,
              Overview: movie.Overview,
              Runtime: movie.Runtime,
              Genres: movie.Genres.Select(g => g.Name).ToList(),
              PosterPath: movie.PosterPath,
              MediaType: "movie",
              ReleaseDate: TmdbDateTimeHelper.ParseReleaseDate(movie.ReleaseDate)?.ToString("yyyy-MM-dd")
            );
            return Result.Ok(dto);
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/movie/details-tmdb/{tmdbId}", async (ISender sender, int tmdbId, CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(tmdbId), ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}
