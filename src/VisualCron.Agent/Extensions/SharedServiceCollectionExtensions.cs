using Microsoft.Extensions.DependencyInjection;

namespace VisualCron.Agent.Extensions;

public static class SharedServiceCollectionExtensions
{
    public static IServiceCollection AddSharedLayer(this IServiceCollection services)
    {
        return services;
    }
}
