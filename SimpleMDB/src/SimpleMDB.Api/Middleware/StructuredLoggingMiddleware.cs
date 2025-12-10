using System.Text.Json;

namespace SimpleMDB.Api.Middleware;

public class StructuredLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<StructuredLoggingMiddleware> _logger;

    public StructuredLoggingMiddleware(RequestDelegate next, ILogger<StructuredLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString();

        // Log request
        _logger.LogInformation("Request started: {RequestId} {Method} {Path} {QueryString}",
            requestId, context.Request.Method, context.Request.Path, context.Request.QueryString);

        await _next(context);

        var duration = DateTime.UtcNow - startTime;

        // Log response
        _logger.LogInformation("Request completed: {RequestId} {StatusCode} in {Duration}ms",
            requestId, context.Response.StatusCode, duration.TotalMilliseconds);
    }
}
