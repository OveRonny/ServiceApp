namespace serviceApp.Server.Features.Tmdb.TmdHelpers;

public static class TmdbDateTimeHelper
{
    public static DateTime? ParseReleaseDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr))
            return null;

        return DateTime.TryParse(dateStr, out var d) ? d : (DateTime?)null;
    }
}
