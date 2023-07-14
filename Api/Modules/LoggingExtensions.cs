using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using Serilog.Formatting;

namespace Api.Modules;

public static class LoggingExtensions
{
    public static IApplicationBuilder UseLogging(this IApplicationBuilder app) =>
        app.UseSerilogRequestLogging(
            options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                };
            });

    public static LoggerConfiguration GetConfiguration(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration,
        ITextFormatter? textFormatter = null,
        IEnumerable<IExceptionDestructurer>? destructurers = null)
    {
        loggerConfiguration.ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithExceptionDetails(
                new DestructuringOptionsBuilder().WithDefaultDestructurers()
                    .WithRootName("ExceptionDetail")
                    .WithDestructurers(destructurers ?? Array.Empty<IExceptionDestructurer>()))
            .WriteTo.Debug();

        if (textFormatter != null)
        {
            loggerConfiguration.WriteTo.Console(textFormatter, standardErrorFromLevel: LogEventLevel.Error);
        }
        else
        {
            loggerConfiguration.WriteTo.Console(standardErrorFromLevel: LogEventLevel.Error);
        }

        return loggerConfiguration;
    }
}