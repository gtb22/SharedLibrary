namespace Shared.Http;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using System.Xml.Linq;
using Shared.Config;

public static class HttpUtils
{
    public static async Task StructuredLogging(HttpListenerRequest req,
        HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        var requestId = props["req.id"]?.ToString() ??
            Guid.NewGuid().ToString("n").Substring(0, 12);
        var startUtc = DateTime.UtcNow;
        var method = req.HttpMethod ?? "UNKNOWN";
        var url = req.Url!.OriginalString ?? req.Url!.ToString();
        var remote = req.RemoteEndPoint.ToString() ?? "unknown";
        res.Headers["X-Request-Id"] = requestId;
        try
        {
            await next();
        }
        finally
        {
            var duration = (DateTime.UtcNow - startUtc).TotalNanoseconds;
            var record = new
            {
                timestamp = startUtc.ToString("o"),
                requestId,
                method,
                url,
                remote,
                statusCode = res.StatusCode,
                contentType = res.ContentType,
                contentLength = res.ContentLength64,
                duration
            };
            Console.WriteLine(JsonSerializer.Serialize(record,
                JsonSerializerOptions.Web));
        }
    }

    public static async Task CentralizedErrorHandling(HttpListenerRequest req,
        HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch(Exception e)
        {
            int code = (int) HttpStatusCode.InternalServerError;
            string message =
                Environment.GetEnvironmentVariable("DEPLOYMENT_MODE") == "production"
                ? "An unexpected error occurred." : e.ToString();
            await SendResponse(req, res, props, code, message, "text/plain");
        }
    }

    public static async Task SendResponse(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, int statusCode, object data, string contentType)
    {
        res.StatusCode = statusCode;
        res.ContentType = contentType;
        if (data is string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            res.ContentLength64 = buffer.Length;
            await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        else
        {
            var json = JsonSerializer.Serialize(data, JsonSerializerOptions.Web);
            var buffer = Encoding.UTF8.GetBytes(json);
            res.ContentLength64 = buffer.Length;
            await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        res.Close();
    }

    /// <summary>
    /// Default response middleware. This middleware performs post-processing: it runs
    /// the rest of the pipeline first and, if no response has been sent by downstream
    /// middleware/controllers (indicated by `HttpRouter.RESPONSE_NOT_SENT`), it sets
    /// the response to 404 Not Found and closes the response.
    /// </summary>
    /// <remarks>
    /// Install this after logging and error-handling middleware to ensure the router
    /// always returns a response for unmatched routes. Usage example:
    /// <code>
    /// var router = new HttpRouter();
    /// router.Use(HttpUtils.StructuredLogging);
    /// router.Use(HttpUtils.CentralizedErrorHandling);
    /// router.Use(HttpUtils.DefaultResponse);
    /// router.Use(HttpUtils.ServeStaticFiles);
    /// router.MapGet("/", AuthController.LandingPage);
    /// </code>
    /// </remarks>
    public static async Task DefaultResponse(HttpListenerRequest req,
        HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        await next();
        if (res.StatusCode == HttpRouter.RESPONSE_NOT_SENT)
        {
            res.StatusCode = (int)HttpStatusCode.NotFound;
            res.StatusDescription = "Not Found";
            res.Close();
        }
    }
}
