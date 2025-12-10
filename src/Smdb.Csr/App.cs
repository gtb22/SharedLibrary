using Shared.Http;

namespace Smdb.Csr;

public class App
{
    private HttpServer? _server;
    private HttpRouter? _router;

    public void ConfigureRoutes()
    {
        _router = new HttpRouter();

        _router.MapGet("/", async (req, res, props) =>
        {
            res.StatusCode = 307;
            res.Headers["Location"] = "/index.html";
            await res.WriteAsync("");
        });
    }

    public void ConfigureMiddleware()
    {
        if (_router == null) throw new InvalidOperationException("Routes not configured");

        _router.Use(HttpUtils.StructuredLogging);
        _router.Use(HttpUtils.CentralizedErrorHandling);
        _router.Use(HttpUtils.AddResponseCorsHeaders);
        _router.Use(HttpUtils.ParseRequestUrl);
        _router.Use(HttpUtils.ParseRequestQueryString);
        _router.Use(HttpUtils.ServeStaticFiles);
        _router.Use(HttpUtils.DefaultResponse);
    }

    public async Task StartAsync(string host, int port)
    {
        ConfigureRoutes();
        ConfigureMiddleware();

        _server = new HttpServer(_router ?? throw new InvalidOperationException("Router not configured"));
        await _server.StartAsync(host, port);
    }
}
