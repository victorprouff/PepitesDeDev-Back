using Core.NuggetAggregate;

namespace Api.Modules;

public static class ApplicationExtensions
{
    public static IServiceCollection RegisterInjection(this IServiceCollection services)
    {
        services.AddTransient<INuggetDomain, NuggetDomain>();

        return services;
    }
}