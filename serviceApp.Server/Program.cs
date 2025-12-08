using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using serviceApp.Server.Features.Autentication;
using serviceApp.Server.Features.Images;
using serviceApp.Server.Features.Roles;
using serviceApp.Server.Startup;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://todo.progorb.no",
            "https://localhost:7179",
            "https://localhost:7119"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDependency(builder.Configuration);
builder.Services.AddAuthenticationSetup(builder.Configuration);


var app = builder.Build();

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

app.MapImageUpload();

app.MapControllers();

app.RegisterEndpointDefinitions();

using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await RoleSeeder.SeedAsync(roleMgr, userMgr, cfg);
}

app.Run();
