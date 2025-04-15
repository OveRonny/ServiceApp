using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ServiceApp.UI;
using ServiceApp.UI.Services.ConsumptionRecordServices;
using ServiceApp.UI.Services.Owners;
using ServiceApp.UI.Services.ServiceTypeServices;
using ServiceApp.UI.Services.VehicleServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7119/") });

builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IConsumptionRecordService, ConsumptionRecordService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IServiceTypeService, ServiceTypeService>();

await builder.Build().RunAsync();
