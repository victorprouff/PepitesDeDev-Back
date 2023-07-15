using Amazon.S3;
using Core.Interfaces;
using Core.Models.Authentification;
using Core.NuggetAggregate;
using Core.Services;
using Core.Services.Interfaces;
using Core.UserAggregate;
using Infrastructure.Repositories;
using Infrastructure.Storages;
using NodaTime;

namespace Api.Modules;

public static class ApplicationExtensions
{
    public static IServiceCollection RegisterInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IClock, SystemClock>(_ => SystemClock.Instance);

        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        services.AddTransient<INuggetRepository, NuggetRepository>(_ =>
            new NuggetRepository(configuration.GetConnectionString("PepitesDatabase")));
        services.AddTransient<IUserRepository, UserRepository>(_ =>
            new UserRepository(configuration.GetConnectionString("PepitesDatabase")));

        services.AddTransient<IPasswordEncryptor, PasswordEncryptor>();
        
        services.AddTransient<INuggetDomain, NuggetDomain>(c =>
            new NuggetDomain(
                c.GetRequiredService<IClock>(),
                c.GetRequiredService<ILogger<NuggetDomain>>(),
                c.GetRequiredService<INuggetRepository>(),
                c.GetRequiredService<IUserRepository>(),
                c.GetRequiredService<IFileStorage>(),
                configuration.GetValue<string>("CleverCloudHost")));
        
        services.AddTransient<IUserDomain, UserDomain>();

        services.AddSingleton<IAmazonS3, AmazonS3Client>(_ => new AmazonS3Client(
            configuration.GetValue<string>("CleverCloudAccessKeyId"),
            configuration.GetValue<string>("CleverCloudSecretAccessKey"),
            new AmazonS3Config
            {
                ServiceURL = configuration.GetValue<string>("CleverCloudUrl")
            }
        ));

        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IFileStorage, FileStorage>();

        return services;
    }
}