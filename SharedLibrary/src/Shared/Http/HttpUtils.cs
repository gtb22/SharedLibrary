using System.Collections;
using System.Text;
using System.Text.Json;

namespace Shared.Http;

public static class HttpUtils
{
    public static async Task StructuredLogging(IHttpRequest req, IHttpResponse res, Hashtable props, Func<Task> next)
    {
        var requestId = Guid.NewGuid().ToString().Substring(0, 8);
        props["requestId"] = requestId;

        Console.WriteLine($"[{requestId}] {req.Method} {req.RawPath}");

        var startTime = DateTime.Now;
        await next();
        var elapsed = DateTime.Now - startTime;

        Console.WriteLine($"[{requestId}] -> {res.StatusCode} ({elapsed.TotalMilliseconds:F2}ms)");
    }

    public static async Task CentralizedErrorHandling(IHttpRequest req, IHttpResponse res, Hashtable props, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
            try
            {
                System.IO.File.AppendAllText("api-error.log", DateTime.Now.ToString("o") + "\n" + ex.ToString() + "\n\n");
            }
            catch { }
            res.StatusCode = 500;
            await res.WriteAsync(JsonSerializer.Serialize(new { success = false, message = "Internal Server Error" }));
        }
    }

    public static async Task AddResponseCorsHeaders(IHttpRequest req, IHttpResponse res, Hashtable props, Func<Task> next)
    {
        res.Headers["Access-Control-Allow-Origin"] = "*";
        res.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
        res.Headers["Access-Control-Allow-Headers"] = "Content-Type";

        if (req.Method == "OPTIONS")
        {
            res.StatusCode = 204;
            await res.WriteAsync("");
            return;
        }

        await next();
    }

    public static async Task ParseRequestUrl(IHttpRequest req, IHttpResponse res, Hashtable props, Func<Task> next)
    {
        await next();
    }

    public static async Task ParseRequestQueryString(IHttpRequest req, IHttpResponse res, Hashtable props, Func<Task> next)
    {
        var queryIndex = req.RawPath.IndexOf('?');
        if (queryIndex >= 0)
        {
            req.QueryString = ParseQueryString(req.RawPath[(queryIndex + 1)..]);
        }

        await next();
    }

    public static async Task ReadRequestBodyAsText(IHttpRequest req, IHttpResponse res, Hashtable props, Func<Task> next)
    {
        if (req.Body != null && req.Body.Length > 0)
        {
            props["bodyText"] = Encoding.UTF8.GetString(req.Body);
        }

        await next();
    }

    public static async Task ReadRequestBodyAsJson(IHttpRequest req, IHttpResponse res, Hashtable props, Func<Task> next)
    {
        if (req.Body != null && req.Body.Length > 0 && IsJsonContent(req))
        {
            try
            {
                var bodyText = Encoding.UTF8.GetString(req.Body);
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(bodyText);
                props["jsonBody"] = jsonElement;
            }
            catch (Exception ex)
            {
                props["jsonParseError"] = ex.Message;
            }
        }

        await next();
    }

    public static async Task ServeStaticFiles(IHttpRequest req, IHttpResponse res, Hashtable props, Func<Task> next)
    {
        var rootDir = GetConfigValue("root.dir", "./wwwroot");
        
        // Make path absolute if relative
        if (!Path.IsPathRooted(rootDir))
        {
            // Start from AppContext.BaseDirectory and navigate to find wwwroot
            var baseDir = new DirectoryInfo(AppContext.BaseDirectory ?? Environment.CurrentDirectory);
            DirectoryInfo? wwwrootDir = null;
            
            // Go up from bin/Debug/net8.0 to find the solution root wwwroot
            // We keep going until we find a wwwroot that contains index.html
            for (int i = 0; i < 5 && baseDir != null; i++)
            {
                var potentialWwwroot = Path.Combine(baseDir.FullName, "wwwroot");
                if (Directory.Exists(potentialWwwroot) && File.Exists(Path.Combine(potentialWwwroot, "index.html")))
                {
                    wwwrootDir = new DirectoryInfo(potentialWwwroot);
                    break;
                }
                baseDir = baseDir.Parent;
            }
            
            if (wwwrootDir != null)
            {
                rootDir = wwwrootDir.FullName;
            }
            else
            {
                // Fallback: continue walking up from the last baseDir to find solution root wwwroot
                while (baseDir != null)
                {
                    var fallbackWwwroot = Path.Combine(baseDir.FullName, "wwwroot");
                    if (Directory.Exists(fallbackWwwroot) && File.Exists(Path.Combine(fallbackWwwroot, "index.html")))
                    {
                        rootDir = fallbackWwwroot;
                        break;
                    }
                    baseDir = baseDir.Parent;
                }
                // If still not found, try current directory
                if (!File.Exists(Path.Combine(rootDir, "index.html")))
                {
                    rootDir = Path.Combine(Environment.CurrentDirectory, "wwwroot");
                }
            }
        }
        
        var filePath = Path.Combine(rootDir, req.Path.TrimStart('/'));
        // Normalize the path to avoid issues with ./ or similar
        filePath = Path.GetFullPath(filePath);

        if (File.Exists(filePath))
        {
            var content = await File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(filePath);
            res.Headers["Content-Type"] = contentType;
            res.StatusCode = 200;
            await res.WriteAsync(content);
            return;
        }

        await next();
    }

    public static async Task DefaultResponse(IHttpRequest req, IHttpResponse res, Hashtable props, Func<Task> next)
    {
        await next();

        if (!res.HeadersSent)
        {
            res.StatusCode = 404;
            await res.WriteAsync(JsonSerializer.Serialize(new { success = false, message = "Not Found" }));
        }
    }

    public static async Task Json(IHttpResponse res, object obj, int statusCode = 200)
    {
        res.StatusCode = statusCode;
        res.Headers["Content-Type"] = "application/json";
        var json = JsonSerializer.Serialize(obj);
        await res.WriteAsync(json);
    }

    private static Dictionary<string, string> ParseQueryString(string query)
    {
        var result = new Dictionary<string, string>();
        var pairs = query.Split('&');
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=', 2);
            if (parts.Length == 2)
            {
                var key = Uri.UnescapeDataString(parts[0]);
                var value = Uri.UnescapeDataString(parts[1]);
                result[key] = value;
            }
        }
        return result;
    }

    private static bool IsJsonContent(IHttpRequest req)
    {
        return req.Headers?.TryGetValue("Content-Type", out var contentType) == true && contentType.Contains("application/json");
    }

    private static string GetContentType(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".html" => "text/html; charset=utf-8",
            ".js" => "application/javascript",
            ".css" => "text/css",
            ".json" => "application/json",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }

    private static string GetConfigValue(string key, string defaultValue)
    {
        try
        {
            var lines = File.ReadAllLines("appsettings.cfg");
            var line = lines.FirstOrDefault(l => l.StartsWith(key + "="));
            if (line != null)
            {
                return line.Substring(key.Length + 1);
            }
        }
        catch { }
        return defaultValue;
    }
}
