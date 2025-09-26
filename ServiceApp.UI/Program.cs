using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ServiceApp.UI;
using ServiceApp.UI.Services;
using ServiceApp.UI.Services.ConsumptionRecordServices;
using ServiceApp.UI.Services.ImageUploadServices;
using ServiceApp.UI.Services.InsuranceServices;
using ServiceApp.UI.Services.MileagehistoryServices;
using ServiceApp.UI.Services.Owners;
using ServiceApp.UI.Services.ServiceCompanyServices;
using ServiceApp.UI.Services.ServiceRecordServices;
using ServiceApp.UI.Services.ServiceTypeServices;
using ServiceApp.UI.Services.SupplierServices;
using ServiceApp.UI.Services.UserServices;
using ServiceApp.UI.Services.VehicleInventoryServices;
using ServiceApp.UI.Services.VehicleServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


var apiBase = builder.HostEnvironment.IsDevelopment()
    ? new Uri("https://localhost:7119/")
    : new Uri("https://progorb.azurewebsites.net/");


// Plain API client
builder.Services.AddHttpClient("Api", c => c.BaseAddress = apiBase);

// Auth
builder.Services.AddAuthorizationCore(options =>
{
    // Match server names if you use them in the UI
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
    options.AddPolicy("FamilyOwner", p => p.RequireRole("FamilyOwner"));
    options.AddPolicy("FamilyGuest", p => p.RequireRole("FamilyGuest"));
    options.AddPolicy("OwnerAdmin", p => p.RequireRole("OwnerAdmin"));

    // Optional: grouped policies mirrored from server
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("FamilyAdmin", p => p.RequireRole("Admin", "FamilyOwner", "OwnerAdmin"));
    options.AddPolicy("OwnerOnly", p => p.RequireRole("OwnerAdmin"));
});
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
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInsuranceService, InsuranceService>();
builder.Services.AddScoped<IMileageHistoryService, MileageHistoryService>();

var host = builder.Build();

await host.SetDefaultCulture("nb-NO");
await host.RunAsync();


