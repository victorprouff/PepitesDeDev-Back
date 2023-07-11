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
        
        services.AddTransient<INuggetDomain, NuggetDomain>();
        services.AddTransient<IUserDomain, UserDomain>();

        
        services.AddScoped<IAmazonS3, AmazonS3Client>(_ => new AmazonS3Client(
            configuration.GetValue<string>("CleverCloud:AccessKeyId"),
            configuration.GetValue<string>("CleverCloud:SecretAccessKey"),
            new AmazonS3Config
            {
                ServiceURL = configuration.GetValue<string>("CleverCloud:Url")
            }
        ));

        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IFileStorage, FileStorage>();

        return services;
    }
}