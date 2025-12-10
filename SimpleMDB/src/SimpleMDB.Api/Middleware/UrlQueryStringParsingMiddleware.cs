namespace SimpleMDB.Api.Middleware;

public class UrlQueryStringParsingMiddleware
{
    private readonly RequestDelegate _next;

    public UrlQueryStringParsingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Parse query strings and make them available
        // For now, just pass through
        await _next(context);
    }
}
