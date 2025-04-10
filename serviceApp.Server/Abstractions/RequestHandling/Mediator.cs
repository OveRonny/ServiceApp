using System.Reflection;

namespace serviceApp.Server.Abstractions.RequestHandling;

public static class Mediator
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        services.AddScoped<ISender, Sender>();

        var handlerInterfaceType = typeof(IRequestHandler<,>);

        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
            .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handlerType in handlerTypes)
        {
            services.AddScoped(handlerType.Interface, handlerType.Implementation);
        }

        return services;
    }
}
