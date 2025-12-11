using System.Text;

namespace Shared.Http;

public class SimpleHttpListenerRequest : HttpRequest
{
    public string Method { get; }
    public string RawPath { get; }
    public string Path { get; }
    public Dictionary<string, string>? QueryString { get; set; }
    public Dictionary<string, string>? Headers { get; }
    public byte[]? Body { get; }
    public Dictionary<string, object>? PathParams { get; set; }

    public SimpleHttpListenerRequest(
        string method,
        string rawPath,
        Dictionary<string, string> headers,
        byte[]? body)
    {
        Method = method;
        RawPath = rawPath;
        Path = ExtractPath(rawPath);
        Headers = headers;
        Body = body;
    }

    private static string ExtractPath(string rawPath)
    {
        var questionMarkIndex = rawPath.IndexOf('?');
        return questionMarkIndex >= 0 ? rawPath[..questionMarkIndex] : rawPath;
    }
}
