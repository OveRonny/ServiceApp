using serviceApp.Server.Features.Autentication;
using serviceApp.Server.Features.Emails;

namespace serviceApp.Server.Startup;

public static class DependencyInjection
{
    public static IServiceCollection AddDependency(this IServiceCollection services, IConfiguration configuration)
    {

        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddMediator();

        services.AddOptions<EmailOptions>()
        .Bind(configuration.GetSection("EmailSettings"))
        .PostConfigure(o =>
        {
            // Fallback from legacy keys if present (only if they are non-empty)
            var legacyHost = configuration["EmailSettings:SMTPServer"];
            if (string.IsNullOrWhiteSpace(o.Host) && !string.IsNullOrWhiteSpace(legacyHost))
                o.Host = legacyHost;

            var legacyUser = configuration["EmailSettings:Username"];
            if (string.IsNullOrWhiteSpace(o.User) && !string.IsNullOrWhiteSpace(legacyUser))
                o.User = legacyUser;

            // Sensible defaults
            if (string.IsNullOrWhiteSpace(o.FromEmail) && !string.IsNullOrWhiteSpace(o.User))
                o.FromEmail = o.User;

            // Port 465 => implicit SSL
            if (o.Port == 465) o.UseStartTls = false;
        })
        .Validate(o => !string.IsNullOrWhiteSpace(o.FromEmail), "EmailSettings:FromEmail is required")
        .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "EmailSettings:Host is required")
        .Validate(o => o.Port > 0, "EmailSettings:Port must be > 0")
        .ValidateOnStart();


        services.AddScoped<ISmtpEmailSender, SmtpEmailSender>();
        services.AddScoped<IRegistrationService, RegistrationService>();

        return services;
    }
}
