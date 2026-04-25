using serviceApp.Server.Features.Tmdb.TmdHelpers;
using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Movies;

public static class ImportMovie
{
    public record Command(int TmdbId, DateTime Date) : ICommand<Response>;

    public record Response(int MediaItemId);

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

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await _db.MediaItems.AnyAsync(x => x.TmdbId == request.TmdbId, cancellationToken))
                return Result.Fail<Response>("Movie already imported");

            var movie = await _tmdb.GetMovieAsync(request.TmdbId);
            if (movie == null)
                return Result.Fail<Response>("Can't get the movie");

            var media = await CreateMediaItemFromTmdb(movie, cancellationToken);

            _db.MediaItems.Add(media);
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Ok(new Response(media.Id));
        }


        private async Task<MediaItem> CreateMediaItemFromTmdb(TmdbMovieDetailsDto movie, CancellationToken ct)
        {
            var media = new MediaItem
            {
                Title = movie.Title,
                Type = MediaType.Movie,
                DurationMinutes = movie.Runtime,
                TmdbId = movie.TmdbId,
                ImdbId = movie.ImdbId,
                ReleaseDate = TmdbDateTimeHelper.ParseReleaseDate(movie.ReleaseDate),
            };

            foreach (var g in movie.Genres)
            {
                var genre = await _db.Genres.FirstOrDefaultAsync(x => x.Name == g.Name, ct)
                            ?? new Genre { Name = g.Name };

                media.MediaItemGenres.Add(new MediaItemGenre { Genre = genre });
            }

            return media;
        }


    }


    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("/api/tmdb/import/movie", async (ISender sender, Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization();
        }
    }
}

