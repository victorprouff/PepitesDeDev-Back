using Core.Interfaces;
using Core.Models.Authentification;
using Core.NuggetAggregate;
using Core.Services;
using Core.Services.Interfaces;
using Core.UserAggregate;
using Infrastructure.Repositories;
using NodaTime;

namespace Api.Modules;

public static class ApplicationExtensions
{
    public static IServiceCollection RegisterInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IClock, SystemClock>(_ => SystemClock.Instance);

        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        services.AddTransient<INuggetRepository, NuggetRepository>(_ =>
            new NuggetRepository(Environment.GetEnvironmentVariable("ConnectionStrings__PepitesDatabase")));
        services.AddTransient<IUserRepository, UserRepository>(_ =>
            new UserRepository(Environment.GetEnvironmentVariable("ConnectionStrings__PepitesDatabase")));
        
        services.AddTransient<IPasswordEncryptor, PasswordEncryptor>();
        
        services.AddTransient<INuggetDomain, NuggetDomain>();
        services.AddTransient<IUserDomain, UserDomain>();

        services.AddScoped<IJwtService, JwtService>(_ =>
            new JwtService(Environment.GetEnvironmentVariable("AppSettings__Secret")));
        
        return services;
    }
}