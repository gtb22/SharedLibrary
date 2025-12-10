using Smdb.Core.Actors;
using Shared.Http;
using Shared.Http.Results;
using System.Collections;

namespace Smdb.Api.Controllers;

public class ActorsController
{
    private readonly ActorService _service;

    public ActorsController(ActorService service)
    {
        _service = service;
    }

    public async Task GetAll(IHttpRequest req, IHttpResponse res, Hashtable props)
    {
        var page = int.TryParse(req.QueryString?["page"], out var p) ? p : 1;
        var size = int.TryParse(req.QueryString?["size"], out var s) ? s : 5;

        var result = await _service.GetAllAsync(page, size);
        await JsonUtils.SendPagedResultResponse(res, result);
    }

    public async Task GetById(IHttpRequest req, IHttpResponse res, Hashtable props)
    {
        if (!int.TryParse(req.PathParams?["id"]?.ToString(), out var id))
        {
            await JsonUtils.SendError(res, "Invalid id", 400);
            return;
        }

        var result = await _service.GetByIdAsync(id);
        await JsonUtils.SendResultResponse(res, result);
    }

    public async Task Create(IHttpRequest req, IHttpResponse res, Hashtable props)
    {
        var body = props["jsonBody"] as System.Text.Json.JsonElement?;
        if (body == null)
        {
            await JsonUtils.SendError(res, "Invalid request body", 400);
            return;
        }

        var actor = new Actor
        {
            Name = body.Value.GetProperty("name").GetString() ?? "",
            BirthYear = body.Value.GetProperty("birthYear").GetInt32(),
            Bio = body.Value.TryGetProperty("bio", out var b) ? b.GetString() ?? "" : ""
        };

        var result = await _service.CreateAsync(actor);
        await JsonUtils.SendResultResponse(res, result, result.Success ? 201 : 400);
    }

    public async Task Update(IHttpRequest req, IHttpResponse res, Hashtable props)
    {
        if (!int.TryParse(req.PathParams?["id"]?.ToString(), out var id))
        {
            await JsonUtils.SendError(res, "Invalid id", 400);
            return;
        }

        var body = props["jsonBody"] as System.Text.Json.JsonElement?;
        if (body == null)
        {
            await JsonUtils.SendError(res, "Invalid request body", 400);
            return;
        }

        var actor = new Actor
        {
            Id = id,
            Name = body.Value.GetProperty("name").GetString() ?? "",
            BirthYear = body.Value.GetProperty("birthYear").GetInt32(),
            Bio = body.Value.TryGetProperty("bio", out var b) ? b.GetString() ?? "" : ""
        };

        var result = await _service.UpdateAsync(actor);
        await JsonUtils.SendResultResponse(res, result);
    }

    public async Task Delete(IHttpRequest req, IHttpResponse res, Hashtable props)
    {
        if (!int.TryParse(req.PathParams?["id"]?.ToString(), out var id))
        {
            await JsonUtils.SendError(res, "Invalid id", 400);
            return;
        }

        var result = await _service.DeleteAsync(id);
        await JsonUtils.SendResultResponse(res, result);
    }
}
