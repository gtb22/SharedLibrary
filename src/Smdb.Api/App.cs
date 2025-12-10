using Smdb.Core.Db;
using Smdb.Core.Movies;
using Smdb.Core.Actors;
using Smdb.Core.Users;
using Shared.Http;
using Shared.Http.Results;
using Smdb.Api.Controllers;
using Smdb.Api.Routers;

namespace Smdb.Api;

public class App
{
    private HttpServer? _server;
    private HttpRouter? _router;

    public void ConfigureServices()
    {
        // Initialize database (singleton)
        _ = MemoryDatabase.Instance;
    }

    public void ConfigureRoutes()
    {
        _router = new HttpRouter();

        // Create repositories
        var movieRepo = new MemoryMovieRepository();
        var actorRepo = new MemoryActorRepository();
        var userRepo = new MemoryUserRepository();

        // Create services
        var movieService = new MovieService(movieRepo);
        var actorService = new ActorService(actorRepo);

        // Create controllers
        var moviesController = new MoviesController(movieService);
        var actorsController = new ActorsController(actorService);

        // Create routers
        var moviesRouter = new MoviesRouter(moviesController);
        var actorsRouter = new ActorsRouter(actorsController);

        // Configure API routes
        var apiRouter = new HttpRouter();
        apiRouter.UseRouter("/movies", moviesRouter);
        apiRouter.UseRouter("/actors", actorsRouter);

        _router.UseRouter("/api/v1", apiRouter);

        // Configure static route
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
        _router.Use(HttpUtils.ReadRequestBodyAsText);
        _router.Use(HttpUtils.ReadRequestBodyAsJson);
        _router.Use(HttpUtils.ServeStaticFiles);
        _router.Use(HttpUtils.DefaultResponse);
    }

    public async Task StartAsync(string host, int port)
    {
        ConfigureServices();
        ConfigureRoutes();
        ConfigureMiddleware();

        _server = new HttpServer(_router ?? throw new InvalidOperationException("Router not configured"));
        await _server.StartAsync(host, port);
    }
}
