using System.Security.Claims;

namespace serviceApp.Server.Features.Tmdb.Dashboard;

public static class GetDashboard
{
    public record Query : IQuery<DashboardDto>;

    public record DashboardDto(
        MovieStatsDto Movies,
        TvStatsDto TvSeries,
        VehicleStatsDto Vehicles,
        List<RecentWatchDto> RecentWatches
    );

    // ── Movies ──────────────────────────────────────────────────────────────
    public record MovieStatsDto(
        int TotalWatched,
        int TotalInWatchlist,
        int TotalHoursWatched
    );

    // ── TV Series ────────────────────────────────────────────────────────────
    public record TvStatsDto(
        int TotalInWatchlist,
        int TotalSeasonsWatched,
        int TotalEpisodesWatched
    );

    // ── Vehicles ─────────────────────────────────────────────────────────────
    public record VehicleStatsDto(
        int TotalVehicles,
        List<VehicleSummaryDto> Summaries
    );

    public record VehicleSummaryDto(
        int Id,
        string Make,
        string Model,
        int Year,
        string LicensePlate,
        int? LatestMileage,
        DateTime? LastFuelDate,
        decimal? LastFuelLiters,
        DateTime? LastServiceDate,
        string? LastServiceType,
        int? LastServiceMileage,
        string? InsuranceCompany,
        int? InsuranceAnnualLimit,
        int? InsuranceUsedKm,
        int? InsuranceRemainingKm
    );

    // ── Recent watches ───────────────────────────────────────────────────────
    public record RecentWatchDto(
        int TmdbId,
        string Title,
        string? PosterPath,
        DateTime? WatchDate,
        string Type
    );

    public class Handler : IQueryHandler<Query, DashboardDto>
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;

        public Handler(ApplicationDbContext db, IHttpContextAccessor httpContext)
        {
            _db = db;
            _httpContext = httpContext;
        }

        public async Task<Result<DashboardDto>> Handle(Query request, CancellationToken ct)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // ── Movies ───────────────────────────────────────────────────────
            var movieWatchedIds = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.MediaItem.Type == MediaType.Movie && w.EpisodeId == null && w.SeasonId == null)
                .Select(w => w.MediaItemId)
                .Distinct()
                .ToListAsync(ct);

            var totalMinutesWatched = await _db.MediaItems
                .AsNoTracking()
                .Where(m => movieWatchedIds.Contains(m.Id))
                .SumAsync(m => (int?)m.DurationMinutes ?? 0, ct);

            var movieWatchlistCount = await _db.WatchlistItems
                .AsNoTracking()
                .CountAsync(w => w.UserId == userId && w.MediaItem.Type == MediaType.Movie, ct);

            var movieStats = new MovieStatsDto(
                TotalWatched: movieWatchedIds.Count,
                TotalInWatchlist: movieWatchlistCount,
                TotalHoursWatched: totalMinutesWatched / 60
            );

            // ── TV Series ────────────────────────────────────────────────────
            var tvWatchlistCount = await _db.WatchlistItems
                .AsNoTracking()
                .CountAsync(w => w.UserId == userId && w.MediaItem.Type == MediaType.Series, ct);

            var tvSeasonsWatched = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.SeasonId != null)
                .Select(w => w.SeasonId)
                .Distinct()
                .CountAsync(ct);

            var tvEpisodesWatched = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.EpisodeId != null)
                .Select(w => w.EpisodeId)
                .Distinct()
                .CountAsync(ct);

            var tvStats = new TvStatsDto(
                TotalInWatchlist: tvWatchlistCount,
                TotalSeasonsWatched: tvSeasonsWatched,
                TotalEpisodesWatched: tvEpisodesWatched
            );

            // ── Vehicles ─────────────────────────────────────────────────────
            var vehicles = await _db.Vehicles
                .AsNoTracking()
                .Include(v => v.MileageHistories)
                .ToListAsync(ct);

            var vehicleIds = vehicles.Select(v => v.Id).ToList();

            List<ServiceRecord> allServiceRecords = [];
            List<ConsumptionRecord> allConsumptionRecords = [];

            if (vehicleIds.Count > 0)
            {
                allServiceRecords = await _db.ServiceRecords
                    .AsNoTracking()
                    .Include(s => s.ServiceType)
                    .Include(s => s.MileageHistory)
                    .Where(s => vehicleIds.Contains(s.VehicleId))
                    .OrderByDescending(s => s.ServiceDate)
                    .ToListAsync(ct);

                // Load consumption records — compute fuel consumption directly in query
                allConsumptionRecords = await _db.ConsumptionRecords
                    .AsNoTracking()
                    .Include(c => c.MileageHistory)
                    .Where(c => vehicleIds.Contains(c.VehicleId))
                    .OrderByDescending(c => c.Date)
                    .ToListAsync(ct);
            }

            var lastServiceByVehicle = allServiceRecords
                .GroupBy(s => s.VehicleId)
                .ToDictionary(g => g.Key, g => g.First());

            List<InsurancePolicy> allInsurancePolicies = [];
            if (vehicleIds.Count > 0)
            {
                allInsurancePolicies = await _db.InsurancePolicies
                    .AsNoTracking()
                    .Include(i => i.Vehicle)
                        .ThenInclude(v => v!.MileageHistories)
                    .Where(i => vehicleIds.Contains(i.VehicleId) && i.IsActive)
                    .ToListAsync(ct);
            }

            var insuranceByVehicle = allInsurancePolicies
                .GroupBy(i => i.VehicleId)
                .ToDictionary(g => g.Key, g => g.First());

            // For fuel consumption: DieselConsumption requires the full vehicle graph.
            // Instead, compute per-vehicle average from raw data grouped in memory.
            var lastConsumptionByVehicle = allConsumptionRecords
                .GroupBy(c => c.VehicleId)
                .ToDictionary(g => g.Key, g => g.First());

            var vehicleSummaries = vehicles.Select(v =>
            {
                var latestMileage = v.MileageHistories
                    .Where(m => m.Type == MileageHistory.MileageType.Kilometerstand)
                    .OrderByDescending(m => m.RecordedDate)
                    .FirstOrDefault()?.Mileage;

                lastConsumptionByVehicle.TryGetValue(v.Id, out var lastFuel);
                lastServiceByVehicle.TryGetValue(v.Id, out var lastService);

                insuranceByVehicle.TryGetValue(v.Id, out var insurance);
                int? insuranceUsed = null;
                int? insuranceRemaining = null;
                if (insurance is not null)
                {
                    var remaining = insurance.CalculateRemainingMileage(v.MileageHistories);
                    insuranceRemaining = remaining;
                    insuranceUsed = Math.Max(0, insurance.AnnualMileageLimit - remaining);
                }

                return new VehicleSummaryDto(
                    Id: v.Id,
                    Make: v.Make,
                    Model: v.Model,
                    Year: v.Year,
                    LicensePlate: v.LicensePlate,
                    LatestMileage: latestMileage,
                    LastFuelDate: lastFuel?.Date,
                    LastFuelLiters: lastFuel?.DieselAdded,
                    LastServiceDate: lastService?.ServiceDate,
                    LastServiceType: lastService?.ServiceType?.Name,
                    LastServiceMileage: lastService?.MileageHistory?.Mileage,
                    InsuranceCompany: insurance?.CompanyName,
                    InsuranceAnnualLimit: insurance?.AnnualMileageLimit,
                    InsuranceUsedKm: insuranceUsed,
                    InsuranceRemainingKm: insuranceRemaining
                );
            }).ToList();

            var vehicleStats = new VehicleStatsDto(
                TotalVehicles: vehicles.Count,
                Summaries: vehicleSummaries
            );

            // Recent watches - deduplicated per media item
            var recentRaw = await _db.WatchHistories
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .Select(w => new
                {
                    w.MediaItemId,
                    w.MediaItem.TmdbId,
                    w.MediaItem.Title,
                    w.MediaItem.PosterPath,
                    w.MediaItem.Type,
                    w.WatchDate
                })
                .GroupBy(w => new { w.MediaItemId, w.TmdbId, w.Title, w.PosterPath, w.Type })
                .Select(g => new
                {
                    g.Key.TmdbId,
                    g.Key.Title,
                    g.Key.PosterPath,
                    g.Key.Type,
                    WatchDate = g.Max(w => w.WatchDate)
                })
                .OrderByDescending(w => w.WatchDate)
                .Take(8)
                .ToListAsync(ct);

            var recentWatches = recentRaw
                .Select(w => new RecentWatchDto(
                    w.TmdbId,
                    w.Title,
                    w.PosterPath,
                    w.WatchDate,
                    w.Type == MediaType.Movie ? "Movie" : "Series"
                ))
                .ToList();

            return Result.Ok(new DashboardDto(movieStats, tvStats, vehicleStats, recentWatches));
        }
    }

    public class Endpoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/dashboard", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(), ct);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }
}
