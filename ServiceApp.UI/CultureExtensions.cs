using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace ServiceApp.UI;

public static class CultureExtensions
{
    public static async Task SetDefaultCulture(this WebAssemblyHost host, string cultureName)
    {
        var js = host.Services.GetRequiredService<IJSRuntime>();
        var result = await js.InvokeAsync<string>("blazorCulture.get");
        var culture = !string.IsNullOrWhiteSpace(result) ? result : cultureName;
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);
    }
}
