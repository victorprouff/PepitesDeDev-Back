using System.Text.Json;
using Core.Exceptions;

namespace Api.Modules;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)((BaseException)error).StatusCode;

            await response.WriteAsync(JsonSerializer.Serialize(new { message = error.Message }));
        }
    }
}