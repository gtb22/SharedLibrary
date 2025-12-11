namespace Shared.Results;

public class Result<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public T? Data { get; set; }
    public int StatusCode { get; set; } = 200;

    public Result() { }

    public Result(Exception ex, int statusCode = 500)
    {
        Success = false;
        Message = ex.Message;
        Data = default;
        StatusCode = statusCode;
    }

    public Result(T data, int statusCode = 200)
    {
        Success = true;
        Data = data;
        StatusCode = statusCode;
    }

    //Renamed factory methods to avoid name collision with the `Success` property.
    public static Result<T> CreateSuccess(T data)
    {
        return new Result<T> { Success = true, Data = data };
    }

    public static Result<T> CreateFailure(string message)
    {
        return new Result<T> { Success = false, Message = message };
    }
}
