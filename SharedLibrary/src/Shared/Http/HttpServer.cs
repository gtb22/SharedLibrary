using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Shared.Http;

public class HttpServer
{
    private HttpRouter _router;
    private TcpListener? _listener;

    public HttpServer(HttpRouter router)
    {
        _router = router;
    }

    public async Task StartAsync(string host, int port)
    {
        var uri = new Uri(host);
        var hostname = uri.Host;

        IPAddress ipAddress;
        if (string.Equals(hostname, "localhost", StringComparison.OrdinalIgnoreCase) || hostname == "127.0.0.1")
        {
            ipAddress = IPAddress.Loopback;
        }
        else if (!IPAddress.TryParse(hostname, out ipAddress))
        {
            var addresses = Dns.GetHostAddresses(hostname);
            ipAddress = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork) ?? addresses.First();
        }

        _listener = new TcpListener(ipAddress, port);
        _listener.Start();

        Console.WriteLine($"[Server] Listening on {host}:{port}");

        try
        {
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await HandleClientAsync(client);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[HandleClient] Error: {ex.Message}");
                    }
                });
            }
        }
        finally
        {
            _listener.Stop();
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        using (var stream = client.GetStream())
        {
            try
            {
                var reader = new StreamReader(stream);
                var requestLine = await reader.ReadLineAsync() ?? "";

                if (string.IsNullOrWhiteSpace(requestLine))
                    return;

                var parts = requestLine.Split(' ');
                if (parts.Length < 2)
                    return;

                var method = parts[0];
                var path = parts[1];

                var headers = new Dictionary<string, string>();
                string? line;
                while ((line = await reader.ReadLineAsync()) != null && !string.IsNullOrEmpty(line))
                {
                    var headerParts = line.Split(':', 2);
                    if (headerParts.Length == 2)
                    {
                        headers[headerParts[0].Trim()] = headerParts[1].Trim();
                    }
                }

                byte[]? body = null;
                if (headers.TryGetValue("Content-Length", out var contentLengthStr) && int.TryParse(contentLengthStr, out var contentLength))
                {
                    body = new byte[contentLength];
                    await stream.ReadExactlyAsync(body, 0, contentLength);
                }

                var request = new SimpleHttpListenerRequest(method, path, headers, body);
                var response = new SimpleHttpListenerResponse(stream);

                var context = new SimpleHttpContext(request, response);
                await _router.HandleContextAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HandleClient] Exception: {ex.Message}");
            }
        }
    }
}
