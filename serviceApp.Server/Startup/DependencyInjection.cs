namespace serviceApp.Server.Startup;

public static class DependencyInjection
{
    public static IServiceCollection AddDependency(this IServiceCollection services, IConfiguration configuration)
    {

        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddMediator();



        return services;
    }
}
