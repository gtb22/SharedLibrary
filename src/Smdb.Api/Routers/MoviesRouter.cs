using Shared.Http;
using Smdb.Api.Controllers;

namespace Smdb.Api.Routers;

public class MoviesRouter : HttpRouter
{
    public MoviesRouter(MoviesController controller)
    {
        MapGet("/", controller.GetAll);
        MapGet("/:id", controller.GetById);
        MapPost("/", controller.Create);
        MapPut("/:id", controller.Update);
        MapDelete("/:id", controller.Delete);
    }
}
