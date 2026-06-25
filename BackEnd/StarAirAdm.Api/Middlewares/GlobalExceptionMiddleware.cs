using System.Net;
using System.Text.Json;

namespace StarAirAdm.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            
            // Catch 400s and 404s that occur without throwing exceptions
            if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                _logger.LogWarning("400 Bad Request returned for {Method} {Path}", context.Request.Method, context.Request.Path);
            }
            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                _logger.LogWarning("404 Not Found returned for {Method} {Path}", context.Request.Method, context.Request.Path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "500 Internal Server Error encountered processing {Method} {Path}. Message: {Message}", 
                context.Request.Method, context.Request.Path, ex.Message);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new 
        {
            StatusCode = context.Response.StatusCode,
            Message = "Exception: " + exception.Message,
            Details = exception.ToString()
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
