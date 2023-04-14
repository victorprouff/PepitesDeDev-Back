using Core.Interfaces;
using Core.NuggetAggregate;
using Core.UserAggregate;
using Infrastructure.Repositories;
using NodaTime;

namespace Api.Modules;

public static class ApplicationExtensions
{
    public static IServiceCollection RegisterInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IClock, SystemClock>(_ => SystemClock.Instance);

        services.AddTransient<INuggetRepository, NuggetRepository>(_ =>
            new NuggetRepository(configuration.GetConnectionString("PepitesDatabase")));
        services.AddTransient<IUserRepository, UserRepository>(_ =>
            new UserRepository(configuration.GetConnectionString("PepitesDatabase")));
        
        services.AddTransient<INuggetDomain, NuggetDomain>();
        services.AddTransient<IUserDomain, UserDomain>();

        return services;
    }
}