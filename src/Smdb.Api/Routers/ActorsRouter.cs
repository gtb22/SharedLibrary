using Shared.Http;
using Smdb.Api.Controllers;

namespace Smdb.Api.Routers;

public class ActorsRouter : HttpRouter
{
    public ActorsRouter(ActorsController controller)
    {
        MapGet("/", controller.GetAll);
        MapGet("/:id", controller.GetById);
        MapPost("/", controller.Create);
        MapPut("/:id", controller.Update);
        MapDelete("/:id", controller.Delete);
    }
}
