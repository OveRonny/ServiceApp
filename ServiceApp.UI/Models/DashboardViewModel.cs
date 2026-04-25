using System.Text.Json.Serialization;

namespace ServiceApp.UI.Models;

public class DashboardViewModel
{
    [JsonPropertyName("movies")]
    public MovieStatsDashboard Movies { get; set; } = new();

    [JsonPropertyName("tvSeries")]
    public TvStatsDashboard TvSeries { get; set; } = new();

    [JsonPropertyName("vehicles")]
    public VehicleStatsDashboard Vehicles { get; set; } = new();

    [JsonPropertyName("recentWatches")]
    public List<RecentWatchDashboard> RecentWatches { get; set; } = new();
}

public class MovieStatsDashboard
{
    [JsonPropertyName("totalWatched")]
    public int TotalWatched { get; set; }

    [JsonPropertyName("totalInWatchlist")]
    public int TotalInWatchlist { get; set; }

    [JsonPropertyName("totalHoursWatched")]
    public int TotalHoursWatched { get; set; }
}

public class TvStatsDashboard
{
    [JsonPropertyName("totalInWatchlist")]
    public int TotalInWatchlist { get; set; }

    [JsonPropertyName("totalSeasonsWatched")]
    public int TotalSeasonsWatched { get; set; }

    [JsonPropertyName("totalEpisodesWatched")]
    public int TotalEpisodesWatched { get; set; }
}

public class VehicleStatsDashboard
{
    [JsonPropertyName("totalVehicles")]
    public int TotalVehicles { get; set; }

    [JsonPropertyName("summaries")]
    public List<VehicleSummaryDashboard> Summaries { get; set; } = new();
}

public class VehicleSummaryDashboard
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("make")]
    public string Make { get; set; } = "";

    [JsonPropertyName("model")]
    public string Model { get; set; } = "";

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("licensePlate")]
    public string LicensePlate { get; set; } = "";

    [JsonPropertyName("latestMileage")]
    public int? LatestMileage { get; set; }

    [JsonPropertyName("lastFuelDate")]
    public DateTime? LastFuelDate { get; set; }

    [JsonPropertyName("lastFuelLiters")]
    public decimal? LastFuelLiters { get; set; }

    [JsonPropertyName("lastServiceDate")]
    public DateTime? LastServiceDate { get; set; }

    [JsonPropertyName("lastServiceType")]
    public string? LastServiceType { get; set; }

    [JsonPropertyName("lastServiceMileage")]
    public int? LastServiceMileage { get; set; }
}

public class RecentWatchDashboard
{
    [JsonPropertyName("tmdbId")]
    public int TmdbId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("posterPath")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("watchDate")]
    public DateTime? WatchDate { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    public string? PosterUrl =>
        string.IsNullOrEmpty(PosterPath) ? null : $"https://image.tmdb.org/t/p/w154{PosterPath}";
}
