using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ServiceApp.UI;
using ServiceApp.UI.Services.ConsumptionRecordServices;
using ServiceApp.UI.Services.Owners;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7119/") });

builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IConsumptionRecordService, ConsumptionRecordService>();

await builder.Build().RunAsync();
