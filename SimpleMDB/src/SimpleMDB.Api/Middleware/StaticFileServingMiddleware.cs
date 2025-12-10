namespace SimpleMDB.Api.Middleware;

public class StaticFileServingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _wwwRootPath;

    public StaticFileServingMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _wwwRootPath = Path.Combine(env.ContentRootPath, "WWWRoot");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.TrimStart('/');
        if (!string.IsNullOrEmpty(path) && !path.StartsWith("api/"))
        {
            var filePath = Path.Combine(_wwwRootPath, path);
            if (File.Exists(filePath))
            {
                var contentType = GetContentType(filePath);
                context.Response.ContentType = contentType;
                await context.Response.SendFileAsync(filePath);
                return;
            }
        }

        await _next(context);
    }

    private string GetContentType(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            _ => "application/octet-stream"
        };
    }
}
