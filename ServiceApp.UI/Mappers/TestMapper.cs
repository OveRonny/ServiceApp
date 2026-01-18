using ServiceApp.UI.Models;

namespace ServiceApp.UI.Mappers;

public static class TestMapper
{
    public static TvDetailsViewModel Map(TvDetailsApiDto dto)
    {
        return new TvDetailsViewModel
        {
            TmdbId = dto.TmdbId,
            Title = dto.Title,
            Overview = dto.Overview,
            FirstAirDate = string.IsNullOrEmpty(dto.FirstAirDate)
                ? null
                : DateTime.Parse(dto.FirstAirDate),
            NumberOfSeasons = dto.NumberOfSeasons,
            NumberOfEpisodes = dto.NumberOfEpisodes,
            Rating = dto.Rating,
            PosterPath = dto.PosterPath,
            Genres = dto.Genres,

            Seasons = dto.Seasons.Select(s => new TvSeasonViewModel
            {
                SeasonNumber = s.SeasonNumber,
                Name = s.Name,
                EpisodeCount = s.EpisodeCount,
                AirDate = s.AirDate,
                PosterPath = s.PosterPath,
                VoteAverage = s.VoteAverage,
                Overview = s.Overview
            }).ToList()
        };
    }

}
