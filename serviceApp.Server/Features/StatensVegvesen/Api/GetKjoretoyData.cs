using serviceApp.Server.Entities.StatensVegvesen.Models;
using System.Linq;

namespace serviceApp.Server.Features.StatensVegvesen.Api;

public static class GetKjoretoyData
{
    public record Query(string RegNr) : IQuery<Response>;
    public record Response(StatensVegvesenDto Data);

    public class Handler(ApiStaten apiStaten) : IQueryHandler<Query, Response>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                var dataList = await apiStaten.GetKjoretoyDataAsync(request.RegNr);
                var kjoretoy = dataList.KjoretoydataListe?.FirstOrDefault();
                if (kjoretoy == null)
                    return Result.Fail<Response>("No vehicle data found");

                var dto = MapToDto(kjoretoy);
                return Result.Ok(new Response(dto));
            }
            catch (Exception ex)
            {
                return Result.Fail<Response>(ex.Message);
            }
        }

        private StatensVegvesenDto MapToDto(Kjoretoydata data)
        {
            var dto = new StatensVegvesenDto();

            if (data != null &&
                data.PeriodiskKjoretoyKontroll != null &&
                data.KjoretoyId != null && data.Registrering != null && data.Godkjenning != null)
            {
                dto.KontrollFrist = data.PeriodiskKjoretoyKontroll.KontrollFrist ?? "default value";
                dto.SistGodkjent = data.PeriodiskKjoretoyKontroll.SistGodkjent ?? "default value";
                dto.UnderstellsNummer = data.KjoretoyId.UnderstellsNummer ?? "default value";
                dto.RegistreringEier = data.Registrering.RegistrertForstegangPaEierskap;
                dto.Dekkdimensjon = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.DekkOgFelg?.AkselDekkOgFelgKombinasjon?.FirstOrDefault()?.AkselDekkOgFelg?.FirstOrDefault()?.Dekkdimensjon ?? "default";
                dto.FelgDimensjon = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.DekkOgFelg?.AkselDekkOgFelgKombinasjon?.FirstOrDefault()?.AkselDekkOgFelg?.FirstOrDefault()?.FelgDimensjon ?? "default value";
                dto.Innpress = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.DekkOgFelg?.AkselDekkOgFelgKombinasjon?.FirstOrDefault()?.AkselDekkOgFelg?.FirstOrDefault()?.Innpress ?? "default value";
                dto.MotorKode = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.MotorOgDrivverk?.Motor?.FirstOrDefault()?.MotorKode ?? "default value";
                dto.KodeNavn = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.MotorOgDrivverk?.Motor?.FirstOrDefault()?.DrivStoff?.FirstOrDefault()?.DrivstoffKode?.KodeNavn ?? "default value";
                dto.Vekt = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.Vekter?.EgenVekt;
                dto.GirKasse = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.MotorOgDrivverk?.GirkasseType?.KodeNavn ?? "default value";
                dto.Lengde = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.Dimensjoner?.Lengde;
                dto.Hoyde = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.Dimensjoner?.Hoyde;
                dto.Bredde = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.Dimensjoner?.Bredde;
                dto.FargeNavn = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.KarosseriOgLasteplan?.RFarge?.FirstOrDefault()?.KodeNavn;
                dto.FargeKode = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.KarosseriOgLasteplan?.RFarge?.FirstOrDefault()?.KodeVerdi;
                dto.FargeBeskrivelse = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.KarosseriOgLasteplan?.RFarge?.FirstOrDefault()?.KodeBeskrivelse;
                var Kw = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.MotorOgDrivverk?.Motor?.FirstOrDefault()?.DrivStoff?.FirstOrDefault()?.MaksNettoEffekt;
                dto.Hestekrefter = Kw.HasValue ? Kw.Value * 1.34102f : (float?)null;
                dto.Seter = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.Persontall?.SittePlasserTotalt;
                dto.Dorer = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.KarosseriOgLasteplan?.AntallDorer?.FirstOrDefault();
                dto.TilhengervektMedBremser = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.Vekter?.TillattTilhengervektMedBrems;
                dto.TilhengervektUtenBremser = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.Vekter?.TillattTilhengervektUtenBrems;
                dto.ForrsteGangRegistrert = data.Forstegangsregistrering?.RegistrertForstegangNorgeDato;
            }

                if (data != null)
            {  
                dto.Merke = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.Generelt?.Merke?.FirstOrDefault()?.MerkeNavn ?? string.Empty;
                dto.Modell = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.Generelt?.HandelsBetegnelse?.FirstOrDefault() ?? string.Empty;


                dto.Ar = data.Kjennemerke?.FirstOrDefault()?.FomTidspunkt.Year ?? 0;
                dto.Registreringsnummer = data.KjoretoyId?.KjenneMerke ?? string.Empty;
                dto.Farge = data.Godkjenning?.TekniskGodkjenning?.TekniskeData?.KarosseriOgLasteplan?.RFarge?.FirstOrDefault()?.KodeNavn ?? string.Empty;
                dto.Eier = ""; // Map if you have owner info
                // ...rest of your mapping
            }


            return dto;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/kjoretoydata/{regNr}", async (ISender sender, string regNr, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(regNr), cancellationToken);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }


}
