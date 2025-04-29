using System.Reflection;

namespace serviceApp.Server.Extensions;

public static class EndpointDefinitionExtensions
{
    public static void RegisterEndpointDefinitions(this WebApplication app)
    {
        var endpointDefinitions = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IEndpointDefinition).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IEndpointDefinition>();

        foreach (var endpointDefinition in endpointDefinitions)
        {
            endpointDefinition.MapEndpoints(app);
        }
    }
}
