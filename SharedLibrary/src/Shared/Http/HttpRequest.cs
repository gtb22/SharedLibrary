namespace Shared.Http;

public interface HttpRequest
{
    string Method { get; }
    string RawPath { get; }
    string Path { get; }
    Dictionary<string, string>? QueryString { get; set; }
    Dictionary<string, string>? Headers { get; }
    byte[]? Body { get; }
    Dictionary<string, object>? PathParams { get; set; }
}
