using System.Text.Json;

namespace SimpleMDB.Api.Middleware;

public class JsonBodyParsingMiddleware
{
    private readonly RequestDelegate _next;

    public JsonBodyParsingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType?.StartsWith("application/json") == true)
        {
            // Enable reading the request body multiple times
            context.Request.EnableBuffering();
        }

        await _next(context);
    }
}
