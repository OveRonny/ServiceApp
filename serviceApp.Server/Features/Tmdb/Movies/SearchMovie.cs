namespace serviceApp.Server.Features.Tmdb.Movies;

public static class SearchMovie
{
    // Query
    public record Query(string SearchQuery) : IQuery<List<Response>>;

    // Response
    public record Response(
        int TmdbId,
        string Title,
        string MediaType,
        int? Year,
        string? PosterPath);

    // Handler
    public class Handler : IQueryHandler<Query, List<Response>>
    {
        private readonly TmdbClient _tmdb;

        public Handler(TmdbClient tmdb)
        {
            _tmdb = tmdb;
        }

        public async Task<Result<List<Response>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var fullResult = await _tmdb.SearchMovieAsync(request.SearchQuery);

            var response = fullResult.Results
               .Select(x => new Response(
                    x.Id,
                    x.Title ?? x.Name ?? "Unknown",
                    x.MediaType,
                    GetYear(x),
                    x.PosterPath
                ))
                .ToList();

            return Result.Ok(response);
        }

        private static int? GetYear(TmdbSearchResultDto x)
        {
            var date = x.ReleaseDate ?? x.FirstAirDate;
            return DateTime.TryParse(date, out var d) ? d.Year : null;
        }
    }

    // Endpoint
    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/search", async (ISender sender, string query, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(query), cancellationToken);
                return Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}


