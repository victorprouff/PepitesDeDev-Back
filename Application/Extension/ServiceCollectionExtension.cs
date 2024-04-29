using Application.Services;
using Core.Interfaces;
using Infrastructure.Repositories;

namespace Application.Extension;

public static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INuggetService, NuggetService>();

        services.AddTransient<INuggetRepository, NuggetRepository>(_ =>
            new NuggetRepository(configuration.GetConnectionString("PepitesDatabase")));
        services.AddTransient<IUserRepository, UserRepository>(_ =>
            new UserRepository(configuration.GetConnectionString("PepitesDatabase")));

        return services;
    }
}