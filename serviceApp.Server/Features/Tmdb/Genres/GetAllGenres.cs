namespace serviceApp.Server.Features.Tmdb.Genres;

public static class GetAllGenres
{
    public record Query : IQuery<List<GenreDto>>;
    public record GenreDto(int Id, string Name);
    public class Handler(ApplicationDbContext db) : IQueryHandler<Query, List<GenreDto>>
    {
        private readonly ApplicationDbContext db = db;

        public async Task<Result<List<GenreDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var genres = await db.Genres.ToListAsync(cancellationToken);
            var genreDtos = genres.Select(g => new GenreDto(g.Id, g.Name)).ToList();
            return Result.Ok(genreDtos);
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/tmdb/genres", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(), ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}
