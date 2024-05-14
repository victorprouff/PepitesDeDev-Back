using Amazon.S3;
using Application.Services;
using Core.Interfaces;
using Core.NuggetAggregate;
using Infrastructure.Repositories;
using Infrastructure.Storages;
using NodaTime;

namespace Application.Extension;

public static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IClock, SystemClock>(_ => SystemClock.Instance);

        services.AddScoped<IFileStorage, FileStorage>();

        services.AddScoped<INuggetService, NuggetService>();

        var connectionString = configuration.GetConnectionString("PepitesDatabase");

        services.AddTransient<INuggetRepository, NuggetRepository>(_ =>
            new NuggetRepository(connectionString));
        services.AddTransient<IUserRepository, UserRepository>(_ =>
            new UserRepository(configuration.GetConnectionString("PepitesDatabase")));

        services.AddTransient<INuggetDomain, NuggetDomain>(c =>
            new NuggetDomain(
                c.GetRequiredService<IClock>(),
                c.GetRequiredService<ILogger<NuggetDomain>>(),
                c.GetRequiredService<INuggetRepository>(),
                c.GetRequiredService<IUserRepository>(),
                c.GetRequiredService<IFileStorage>(),
                configuration.GetValue<string>("CleverCloudHost")));

        services.AddSingleton<IAmazonS3, AmazonS3Client>(_ => new AmazonS3Client(
            configuration.GetValue<string>("CleverCloudAccessKeyId"),
            configuration.GetValue<string>("CleverCloudSecretAccessKey"),
            new AmazonS3Config
            {
                ServiceURL = configuration.GetValue<string>("CleverCloudUrl")
            }
        ));

        return services;
    }
}