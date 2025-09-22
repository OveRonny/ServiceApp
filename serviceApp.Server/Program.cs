using Scalar.AspNetCore;
using serviceApp.Server.Features.Autentication;
using serviceApp.Server.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDependency(builder.Configuration);
builder.Services.AddAuthenticationSetup(builder.Configuration);

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
        .AllowAnyMethod();
    });
});

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

app.MapControllers();

app.RegisterEndpointDefinitions();

app.Run();
