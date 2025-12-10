using System.Text.Json;
using Shared.Results;

namespace Shared.Http.Results;

public static class JsonUtils
{
    public static async Task Json(IHttpResponse res, object obj, int statusCode = 200)
    {
        res.StatusCode = statusCode;
        res.Headers["Content-Type"] = "application/json";
        var json = JsonSerializer.Serialize(obj);
        await res.WriteAsync(json);
    }

    public static async Task SendResultResponse<T>(IHttpResponse res, Result<T> result, int? successStatusCode = null)
    {
        var statusCode = result.Success ? (successStatusCode ?? 200) : 400;
        await Json(res, result, statusCode);
    }

    public static async Task SendPagedResultResponse<T>(IHttpResponse res, PagedResult<T> result)
    {
        await Json(res, result, 200);
    }

    public static async Task SendError(IHttpResponse res, string message, int statusCode = 400)
    {
        var error = new { success = false, message };
        await Json(res, error, statusCode);
    }
}
