using serviceApp.Server.Features.Autentication;
using serviceApp.Server.Features.Emails;
using serviceApp.Server.Features.Images;

namespace serviceApp.Server.Startup;

public static class DependencyInjection
{
    public static IServiceCollection AddDependency(this IServiceCollection services, IConfiguration configuration)
    {

        var cs =
                 configuration.GetConnectionString("DefaultConnection")
                 ?? configuration["ConnectionStrings:DefaultConnection"]
                 ?? Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection")
                 ?? Environment.GetEnvironmentVariable("SQLCONNSTR_DefaultConnection")
                 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(cs, sql =>
            {
                // Transient fault handling for Azure SQL
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            }));

        services.AddMediator();

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection("EmailSettings"))
            .PostConfigure(o =>
            {
                var legacyHost = configuration["EmailSettings:SMTPServer"];
                if (string.IsNullOrWhiteSpace(o.Host) && !string.IsNullOrWhiteSpace(legacyHost))
                    o.Host = legacyHost;

                var legacyUser = configuration["EmailSettings:Username"];
                if (string.IsNullOrWhiteSpace(o.User) && !string.IsNullOrWhiteSpace(legacyUser))
                    o.User = legacyUser;

                if (string.IsNullOrWhiteSpace(o.FromEmail) && !string.IsNullOrWhiteSpace(o.User))
                    o.FromEmail = o.User;

                if (o.Port == 465) o.UseStartTls = false;
            });

        services.AddScoped<ISmtpEmailSender, SmtpEmailSender>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<AzureBlobImageService>();

        return services;
    }
}
