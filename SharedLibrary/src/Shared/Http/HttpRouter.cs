namespace Shared.Http;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Web;

public class HttpRouter
{
    public const int RESPONSE_NOT_SENT = 777;
    private static ulong requestId = 0;
    private string basePath;
    private List<HttpMiddleware> middlewares;
    private List<(string, string, HttpMiddleware[])> routes;

    public HttpRouter()
    {
        basePath = string.Empty;
        middlewares = [];
        routes = [];
    }

    public HttpRouter Use(params HttpMiddleware[] middlewares)
    {
        this.middlewares.AddRange(middlewares);
        return this;
    }

    public HttpRouter Map(string method, string path,
        params HttpMiddleware[] middlewares)
    {
        routes.Add((method.ToUpperInvariant(), path, middlewares));
        return this;
    }

    public HttpRouter MapGet(string path, params HttpMiddleware[] middlewares)
    {
        return Map("GET", path, middlewares);
    }

    public HttpRouter MapPost(string path, params HttpMiddleware[] middlewares)
    {
        return Map("POST", path, middlewares);
    }

    public HttpRouter MapPut(string path, params HttpMiddleware[] middlewares)
    {
        return Map("PUT", path, middlewares);
    }

    public HttpRouter MapDelete(string path, params HttpMiddleware[] middlewares)
    {
        return Map("DELETE", path, middlewares);
    }

    private async Task HandleAsync(HttpListenerRequest req,
        HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        foreach (var (method, path, routeMiddlewares) in routes)
        {
            if (method == req.HttpMethod.ToUpperInvariant())
            {
                string fullPath = basePath + path;
                var parameters = ParseUrlParams(req.Url.AbsolutePath, fullPath);
                if (parameters != null)
                {
                    foreach (string? key in parameters.AllKeys)
                    {
                        if (key != null)
                        {
                            props[key] = parameters[key];
                        }
                    }
                    Func<Task> routePipeline = GenerateMiddlewarePipeline(req, res, props, routeMiddlewares.ToList());
                    await routePipeline();
                    return;
                }
            }
        }

        Func<Task> globalMiddlewarePipeline =
            GenerateMiddlewarePipeline(req, res, props, middlewares);
        await globalMiddlewarePipeline();
        await next();
    }

    public HttpRouter UseRouter(string path, HttpRouter router)
    {
        router.basePath = this.basePath + path;
        return Use(router.HandleAsync);
    }

    private Func<Task> GenerateMiddlewarePipeline(HttpListenerRequest req,
        HttpListenerResponse res, Hashtable props, List<HttpMiddleware> middlewareList)
    {
        Func<Task> pipeline = () => Task.CompletedTask;
        for (int i = middlewareList.Count - 1; i >= 0; i--)
        {
            var middleware = middlewareList[i];
            var next = pipeline;
            pipeline = () => middleware(req, res, props, next);
        }
        return pipeline;
    }

    public static NameValueCollection? ParseUrlParams(string uPath, string rPath)
    {
        string[] uParts = uPath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        string[] rParts = rPath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (uParts.Length != rParts.Length) { return null; }
        var parameters = new NameValueCollection();
        for (int i = 0; i < rParts.Length; i++)
        {
            string uPart = uParts[i];
            string rPart = rParts[i];
            if (rPart.StartsWith(":"))
            {
                string paramName = rPart.Substring(1);
                parameters[paramName] = HttpUtility.UrlDecode(uPart);
            }
            else if (uPart != rPart)
            {
                return null;
            }
        }
        return parameters;
    }

    public async Task HandleContextAsync(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var res = ctx.Response;
        var props = new Hashtable();
        await HandleAsync(req, res, props, async () =>
        {
            if (res.StatusCode == RESPONSE_NOT_SENT)
            {
                res.StatusCode = 404;
                res.StatusDescription = "Not Found";
            }
            res.Close();
        });
    }
}
