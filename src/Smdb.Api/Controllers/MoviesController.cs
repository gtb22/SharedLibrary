using Smdb.Core.Movies;
using Shared.Http;
using Shared.Http.Results;
using System.Collections;

namespace Smdb.Api.Controllers;

public class MoviesController
{
    private readonly MovieService _service;

    public MoviesController(MovieService service)
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

        var movie = new Movie
        {
            Title = body.Value.GetProperty("title").GetString() ?? "",
            Year = body.Value.GetProperty("year").GetInt32(),
            Description = body.Value.GetProperty("description").GetString() ?? "",
            Genre = body.Value.TryGetProperty("genre", out var g) ? g.GetString() ?? "" : "",
            Rating = body.Value.TryGetProperty("rating", out var r) ? r.GetDouble() : 0
        };

        var result = await _service.CreateAsync(movie);
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

        var movie = new Movie
        {
            Id = id,
            Title = body.Value.GetProperty("title").GetString() ?? "",
            Year = body.Value.GetProperty("year").GetInt32(),
            Description = body.Value.GetProperty("description").GetString() ?? "",
            Genre = body.Value.TryGetProperty("genre", out var g) ? g.GetString() ?? "" : "",
            Rating = body.Value.TryGetProperty("rating", out var r) ? r.GetDouble() : 0
        };

        var result = await _service.UpdateAsync(movie);
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
