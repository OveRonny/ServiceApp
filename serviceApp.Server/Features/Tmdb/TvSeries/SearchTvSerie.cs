namespace serviceApp.Server.Features.Tmdb.TvSeries;

public static class SearchTv
{
    public record Query(string SearchQuery) : IQuery<List<Response>>;

    public record Response(
        int TmdbId,
        string Title,
        int? FirstAirYear,
        string? Overview,
        string? PosterUrl
    );

    public class Handler : IQueryHandler<Query, List<Response>>
    {
        private readonly TmdbClient _tmdb;

        public Handler(TmdbClient tmdb) => _tmdb = tmdb;

        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken ct)
        {
            var result = await _tmdb.SearchTvAsync(request.SearchQuery);

            var response = result.Results.Select(x => new Response(
                x.Id,
                x.Name ?? "",
                DateTime.TryParse(x.FirstAirDate, out var d) ? d.Year : null,
                x.Overview,
                x.PosterPath is null ? null : $"https://image.tmdb.org/t/p/w500{x.PosterPath}"
            )).ToList();

            return Result.Ok(response);
        }
    }


    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/search-tv", async (ISender sender, string query, CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(query), ct);
                return Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}

