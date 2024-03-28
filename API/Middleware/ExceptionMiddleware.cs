using System.Net;
using System.Text.Json;
using API.Errors;

namespace API;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    private readonly IHostEnvironment _env = env;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            ApiException response = _env.IsDevelopment()
            ? new(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
            : new(context.Response.StatusCode, ex.Message, "Internal Server Error");

            JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            string json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);

        }
    }

}
