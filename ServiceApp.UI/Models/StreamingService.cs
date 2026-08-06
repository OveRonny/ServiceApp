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
    Other,
    TV2Play,
    BritBox
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
        StreamingService.TV2Play       => "TV 2 Play",
        StreamingService.BritBox       => "BritBox",
        _                              => service.ToString()
    };
}
