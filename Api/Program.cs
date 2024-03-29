using Api.Authorization;
using Api.Modules;
using Microsoft.AspNetCore.Http.Features;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Prometheus;
using Serilog.Formatting.Json;
using Serilog;
using Serilog.Formatting.Compact;

const string allowSpecificOrigin = "AllowSpecificOrigin";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, cfg) =>
{

    cfg.GetConfiguration(context.Configuration, new JsonFormatter(renderMessage: true));
    cfg.Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .WriteTo.Console(new RenderedCompactJsonFormatter());
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(
        o =>
        {
            o.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            // o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowSpecificOrigin,
        policy =>
        {
            policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .RegisterInjection(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

var app = builder.Build();

app.UseMetricServer();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseLogging();

app.UseCors(allowSpecificOrigin);

app.UseHttpsRedirection();

app.UseHttpMetrics(options =>
{
    options.AddCustomLabel("host", context => context.Request.Host.Host);
});

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();