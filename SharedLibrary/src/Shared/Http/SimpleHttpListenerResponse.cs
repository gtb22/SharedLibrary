using System.Net.Sockets;
using System.Text;

namespace Shared.Http;

public class SimpleHttpListenerResponse : IHttpResponse
{
    private NetworkStream _stream;
    private bool _headersSent = false;

    public int StatusCode { get; set; } = 200;
    public Dictionary<string, string> Headers { get; } = new();
    public Stream Stream => _stream;
    public bool HeadersSent => _headersSent;

    public SimpleHttpListenerResponse(NetworkStream stream)
    {
        _stream = stream;
    }

    public async Task WriteAsync(string content)
    {
        await WriteAsync(Encoding.UTF8.GetBytes(content));
    }

    public async Task WriteAsync(byte[] content)
    {
        if (!_headersSent)
        {
            await SendHeadersAsync(content.Length);
        }

        await _stream.WriteAsync(content, 0, content.Length);
        await _stream.FlushAsync();
    }

    private async Task SendHeadersAsync(int contentLength = 0)
    {
        var statusText = GetStatusText(StatusCode);
        var headerLine = $"HTTP/1.1 {StatusCode} {statusText}\r\n";

        Headers["Content-Length"] = contentLength.ToString();

        foreach (var header in Headers)
        {
            headerLine += $"{header.Key}: {header.Value}\r\n";
        }

        headerLine += "\r\n";

        var headerBytes = Encoding.UTF8.GetBytes(headerLine);
        await _stream.WriteAsync(headerBytes, 0, headerBytes.Length);
        await _stream.FlushAsync();

        _headersSent = true;
    }

    private static string GetStatusText(int code) => code switch
    {
        200 => "OK",
        201 => "Created",
        204 => "No Content",
        300 => "Multiple Choices",
        301 => "Moved Permanently",
        302 => "Found",
        304 => "Not Modified",
        307 => "Temporary Redirect",
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        405 => "Method Not Allowed",
        500 => "Internal Server Error",
        _ => "Unknown"
    };
}
