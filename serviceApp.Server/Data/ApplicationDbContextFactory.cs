using Microsoft.EntityFrameworkCore.Design;

namespace serviceApp.Server.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration at design time (appsettings + user secrets + env vars)
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddUserSecrets(typeof(ApplicationDbContextFactory).Assembly, optional: true)
            .Build();

        // Allow overriding via CLI arg or env var; otherwise use configuration ConnectionStrings
        var connectionString =
            (args.Length > 0 ? args[0] : null)
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? configuration["ConnectionStrings:DefaultConnection"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found. Ensure appsettings or user secrets have ConnectionStrings:DefaultConnection, or set ConnectionStrings__DefaultConnection.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });

        var httpContextAccessor = new DesignTimeHttpContextAccessor();
        return new ApplicationDbContext(optionsBuilder.Options, httpContextAccessor);
    }

    // Minimal IHttpContextAccessor for design-time
    private class DesignTimeHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }
}
