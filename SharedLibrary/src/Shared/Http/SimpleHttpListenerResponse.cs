using System.Net.Sockets;
using System.Text;

namespace Shared.Http;

public class SimpleHttpListenerResponse : HttpResponse
{
    private NetworkStream _stream;
    private bool _headersSent;
    

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
        Console.WriteLine($"[Response.WriteAsync] Writing string, length={content?.Length ?? 0}");
        Console.Out.Flush();
        await WriteAsync(Encoding.UTF8.GetBytes(content ?? string.Empty));
    }

    public async Task WriteAsync(byte[] content)
    {
        Console.WriteLine($"[Response.WriteAsync] Writing {content.Length} bytes, headers sent={_headersSent}");
        Console.Out.Flush();
        
        if (!_headersSent)
        {
            Console.WriteLine($"[Response.WriteAsync] Sending headers");
            Console.Out.Flush();
            await SendHeadersAsync(content.Length);
            Console.WriteLine($"[Response.WriteAsync] Headers sent");
            Console.Out.Flush();
        }

        Console.WriteLine($"[Response.WriteAsync] Writing body to stream");
        Console.Out.Flush();
        await _stream.WriteAsync(content, 0, content.Length);
        
        Console.WriteLine($"[Response.WriteAsync] Flushing stream");
        Console.Out.Flush();
        await _stream.FlushAsync();
        
        Console.WriteLine($"[Response.WriteAsync] Complete");
        Console.Out.Flush();
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
