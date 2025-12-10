using System.Collections;
using System.Text.RegularExpressions;

namespace Shared.Http;

public class HttpRouter
{
    private List<(string method, Regex pathRegex, Func<IHttpRequest, IHttpResponse, Hashtable, Task> handler)> _routes = new();
    private List<Func<IHttpRequest, IHttpResponse, Hashtable, Func<Task>, Task>> _middlewares = new();
    private Dictionary<string, HttpRouter> _subrouters = new();

    public string BasePath { get; set; } = "";

    public void MapGet(string path, Func<IHttpRequest, IHttpResponse, Hashtable, Task> handler)
    {
        MapRoute("GET", path, handler);
    }

    public void MapPost(string path, Func<IHttpRequest, IHttpResponse, Hashtable, Task> handler)
    {
        MapRoute("POST", path, handler);
    }

    public void MapPut(string path, Func<IHttpRequest, IHttpResponse, Hashtable, Task> handler)
    {
        MapRoute("PUT", path, handler);
    }

    public void MapDelete(string path, Func<IHttpRequest, IHttpResponse, Hashtable, Task> handler)
    {
        MapRoute("DELETE", path, handler);
    }

    private void MapRoute(string method, string path, Func<IHttpRequest, IHttpResponse, Hashtable, Task> handler)
    {
        var regex = ConvertPathToRegex(path);
        _routes.Add((method, regex, handler));
    }

    public void UseRouter(string path, HttpRouter router)
    {
        router.BasePath = path;
        _subrouters[path] = router;
    }

    public void Use(Func<IHttpRequest, IHttpResponse, Hashtable, Func<Task>, Task> middleware)
    {
        _middlewares.Add(middleware);
    }

    public async Task HandleContextAsync(IHttpContext context)
    {
        var props = context.Properties;
        Func<Task> next = async () =>
        {
            await RouteAsync(context.Request, context.Response, props);
        };

        foreach (var middleware in _middlewares.AsEnumerable().Reverse())
        {
            var currentNext = next;
            next = () => middleware(context.Request, context.Response, props, currentNext);
        }

        await next();
    }

    private async Task RouteAsync(IHttpRequest req, IHttpResponse res, Hashtable props)
    {
        var path = req.Path;
        var method = req.Method;

        Console.WriteLine($"[Router] Routing {method} {path}");

        // Check subrouters first
        foreach (var (prefix, router) in _subrouters)
        {
            Console.WriteLine($"[Router] Checking subrouter prefix '{prefix}' against path '{path}'");
            if (path.StartsWith(prefix + "/") || path == prefix)
            {
                var subPath = path == prefix ? "/" : path[prefix.Length..];
                req.PathParams ??= new();
                var subContext = new SimpleHttpContext(
                    new SimpleHttpListenerRequest(method, subPath, req.Headers ?? new(), req.Body),
                    res
                );
                // copy properties into subcontext
                foreach (System.Collections.DictionaryEntry de in props)
                {
                    subContext.Properties[de.Key] = de.Value;
                }
                await router.HandleContextAsync(subContext);
                return;
            }
        }

        // Check direct routes
        foreach (var (routeMethod, pathRegex, handler) in _routes)
        {
            if (routeMethod == method)
            {
                var match = pathRegex.Match(path);
                if (match.Success)
                {
                    req.PathParams ??= new();
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        var paramName = pathRegex.ToString().Split("(?<")[i]?.Split(">")[0] ?? $"param{i}";
                        req.PathParams[paramName] = match.Groups[i].Value;
                    }

                    await handler(req, res, props);
                    return;
                }
            }
        }
    }

    private static Regex ConvertPathToRegex(string path)
    {
        if (path == "/*")
            return new Regex(".*");

        var pattern = Regex.Escape(path);
        pattern = Regex.Replace(pattern, @"\\:(\w+)", "(?<$1>[^/]+)");
        pattern = $"^{pattern}$";

        return new Regex(pattern);
    }
}
