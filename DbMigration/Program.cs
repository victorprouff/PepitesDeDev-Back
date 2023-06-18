using DbMigration.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .AddFluentMigratorCore()
    .ConfigureRunner(
        rb => rb
            .AddPostgres()
            .WithGlobalConnectionString(Environment.GetEnvironmentVariable("ConnectionStrings__PepitesDatabase"))
            .ScanIn(typeof(InitNuggetTable).Assembly)
            .For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole())
    .BuildServiceProvider(false);

using var scope = serviceProvider.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

runner.MigrateUp();