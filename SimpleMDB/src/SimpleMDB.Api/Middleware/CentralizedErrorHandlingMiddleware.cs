using System.Text.Json;

namespace SimpleMDB.Api.Middleware;

public class CentralizedErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CentralizedErrorHandlingMiddleware> _logger;

    public CentralizedErrorHandlingMiddleware(RequestDelegate next, ILogger<CentralizedErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var errorResponse = new { error = "Internal server error" };
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
