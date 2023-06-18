using Api.Authorization;
using Api.Modules;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

const string allowSpecificOrigin = "AllowSpecificOrigin";

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(allowSpecificOrigin);

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();