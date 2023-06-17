using System.Data;
using Dapper;
using Infrastructure.Handlers;
using Npgsql;

namespace Api.Modules;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddTransient<IDbConnection>(_ => new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__PepitesDatabase")));

        SqlMapper.AddTypeHandler(new InstantHandler());

        return services;
    }
}