using Shared.Http;
using System.Net.Http;
using System.Text;
using System.Collections;

namespace Smdb.Csr;

public class App
{
    private HttpServer? _server;
    private HttpRouter? _router;
    private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
    private const string API_BASE_URL = "http://localhost:5000";

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
        
        // Proxy API requests to the API server (reads body internally)
        _router.Use(async (req, res, props, next) =>
        {
            if (req.Path?.StartsWith("/api/") == true)
            {
                await ProxyToApiServer(req, res, props);
                return;
            }
            await next();
        });
        
        _router.Use(HttpUtils.ServeStaticFiles);
        _router.Use(HttpUtils.DefaultResponse);
    }

    private async Task ProxyToApiServer(IHttpRequest req, IHttpResponse res, Hashtable props)
    {
        try
        {
            var targetUrl = $"{API_BASE_URL}{req.Path}";
            
            // Add query string if present
            if (req.QueryString != null && req.QueryString.Count > 0)
            {
                var queryParams = new List<string>();
                foreach (var key in req.QueryString.Keys)
                {
                    queryParams.Add($"{key}={Uri.EscapeDataString(req.QueryString[key] ?? "")}");
                }
                targetUrl += "?" + string.Join("&", queryParams);
            }

            Console.WriteLine($"[Proxy] {req.Method} {targetUrl}");

            var request = new HttpRequestMessage(
                new HttpMethod(req.Method ?? "GET"),
                targetUrl
            );

            // Copy body for POST/PUT/PATCH from the raw request body
            if (req.Method == "POST" || req.Method == "PUT" || req.Method == "PATCH")
            {
                if (req.Body != null && req.Body.Length > 0)
                {
                    var bodyText = Encoding.UTF8.GetString(req.Body);
                    Console.WriteLine($"[Proxy] Body: {bodyText.Substring(0, Math.Min(100, bodyText.Length))}");
                    request.Content = new StringContent(bodyText, Encoding.UTF8, "application/json");
                }
                else
                {
                    Console.WriteLine("[Proxy] No body found");
                }
            }

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[Proxy] <- {(int)response.StatusCode}");

            // Set response
            res.StatusCode = (int)response.StatusCode;
            res.Headers["Content-Type"] = response.Content.Headers.ContentType?.ToString() ?? "application/json";
            await res.WriteAsync(content);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[Proxy] HTTP Error: {ex.Message}");
            res.StatusCode = 502;
            res.Headers["Content-Type"] = "application/json";
            await res.WriteAsync("{\"error\":\"API server unavailable\"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Proxy] Error: {ex.Message}");
            Console.WriteLine($"[Proxy] Stack: {ex.StackTrace}");
            res.StatusCode = 500;
            res.Headers["Content-Type"] = "application/json";
            await res.WriteAsync($"{{\"error\":\"Proxy error: {ex.Message.Replace("\"", "'")}\"}}");
        }
    }

    public async Task StartAsync(string host, int port)
    {
        ConfigureRoutes();
        ConfigureMiddleware();

        _server = new HttpServer(_router ?? throw new InvalidOperationException("Router not configured"));
        await _server.StartAsync(host, port);
    }
}
