using Scalar.AspNetCore;
using serviceApp.Server.Features.Autentication;
using serviceApp.Server.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDependency(builder.Configuration);
builder.Services.AddAuthenticationSetup(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://todo.progorb.no")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

await IdentitySeeding.EnsureRolesAsync(app.Services);
await IdentitySeeding.EnsureOwnersAdminAsync(app.Services, app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapRegistration();
app.MapIdentityApi<ApplicationUser>();
app.MapFamilyAdmin();

app.MapControllers();

app.RegisterEndpointDefinitions();

app.Run();
