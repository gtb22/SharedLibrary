namespace Shared.Http;

public interface IHttpResponse
{
    int StatusCode { get; set; }
    Dictionary<string, string> Headers { get; }
    Stream Stream { get; }
    bool HeadersSent { get; }
    Task WriteAsync(string content);
    Task WriteAsync(byte[] content);
}
