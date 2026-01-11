using Microsoft.EntityFrameworkCore.Design;

namespace serviceApp.Server.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Get connection string from command line args or environment variable
        var connectionString = args.Length > 0
            ? args[0]
            : Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string not provided. Use --connection parameter or set ConnectionStrings__DefaultConnection environment variable.");
        }

        optionsBuilder.UseSqlServer(connectionString);

        // Create a minimal HttpContextAccessor for design-time
        var httpContextAccessor = new DesignTimeHttpContextAccessor();

        return new ApplicationDbContext(optionsBuilder.Options, httpContextAccessor);
    }

    // Minimal implementation of IHttpContextAccessor for design-time use
    private class DesignTimeHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }
}
