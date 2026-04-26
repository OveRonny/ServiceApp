namespace ServiceApp.UI.Models;

public enum StreamingService
{
    Netflix,
    DisneyPlus,
    HBOMax,
    AmazonPrime,
    AppleTVPlus,
    Viaplay,
    NRKTVPlus,
    TVNorge,
    ParamountPlus,
    Other
}

public static class StreamingServiceExtensions
{
    public static string DisplayName(this StreamingService service) => service switch
    {
        StreamingService.Netflix       => "Netflix",
        StreamingService.DisneyPlus    => "Disney+",
        StreamingService.HBOMax        => "HBO Max",
        StreamingService.AmazonPrime   => "Amazon Prime",
        StreamingService.AppleTVPlus   => "Apple TV+",
        StreamingService.Viaplay       => "Viaplay",
        StreamingService.NRKTVPlus     => "NRK TV",
        StreamingService.TVNorge       => "TV Norge",
        StreamingService.ParamountPlus => "Paramount+",
        StreamingService.Other         => "Annet",
        _                              => service.ToString()
    };
}
