using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ServiceApp.UI;
using ServiceApp.UI.Services;
using ServiceApp.UI.Services.ConsumptionRecordServices;
using ServiceApp.UI.Services.Owners;
using ServiceApp.UI.Services.ServiceCompanyServices;
using ServiceApp.UI.Services.ServiceRecordServices;
using ServiceApp.UI.Services.ServiceTypeServices;
using ServiceApp.UI.Services.SupplierServices;
using ServiceApp.UI.Services.VehicleInventoryServices;
using ServiceApp.UI.Services.VehicleServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Base address for your Server API (adjust if different)
var apiBase = new Uri("https://progorb.azurewebsites.net/");


// Plain API client
builder.Services.AddHttpClient("Api", c => c.BaseAddress = apiBase);

// Auth
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddTransient<BearerHandler>();

// Authed API client (adds Authorization: Bearer <token>)
builder.Services.AddHttpClient("ApiAuthed", c => c.BaseAddress = apiBase)
    .AddHttpMessageHandler<BearerHandler>();

builder.Services.AddBlazoredToast();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IConsumptionRecordService, ConsumptionRecordService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IServiceTypeService, ServiceTypeService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IServiceRecordService, ServiceRecordService>();
builder.Services.AddScoped<IServiceCompanyService, ServiceCompanyService>();
builder.Services.AddScoped<IVehicleInventoryService, VehicleInventoryService>();

await builder.Build().RunAsync();
