namespace ServiceApp.UI.Services.MovieServices;

public record ImportResult(bool Success, string? ErrorMessage = null, int? MovieId = null);
