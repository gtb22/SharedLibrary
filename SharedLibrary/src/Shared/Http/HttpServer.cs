using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
        IPAddress ipAddress;
        
        // Parse the host - could be a URL or an IP address
        if (host.StartsWith("http://") || host.StartsWith("https://"))
        {
            var uri = new Uri(host);
            var hostname = uri.Host;
            if (string.Equals(hostname, "localhost", StringComparison.OrdinalIgnoreCase))
            {
                ipAddress = IPAddress.Loopback;
            }
            else if (!IPAddress.TryParse(hostname, out ipAddress))
            {
                var addresses = Dns.GetHostAddresses(hostname);
                ipAddress = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork) ?? addresses.First();
            }
        }
        else if (IPAddress.TryParse(host, out var parsedIp))
        {
            ipAddress = parsedIp;
        }
        else if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            ipAddress = IPAddress.Loopback;
        }
        else
        {
            var addresses = Dns.GetHostAddresses(host);
            ipAddress = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork) ?? addresses.First();
        }

        _listener = new TcpListener(ipAddress, port);
        _listener.Start();

        Console.WriteLine($"[Server] Listening on http://{host}:{port}");

        try
        {
            Console.WriteLine($"[Server] Entering accept loop");
            while (true)
            {
                Console.WriteLine($"[Server] About to accept client");
                Console.Out.Flush();
                try
                {
                    // Use synchronous Accept with BeginAccept/EndAccept to make it non-blocking
                    var client = _listener.AcceptTcpClient();
                    Console.WriteLine($"[Server] Client accepted, spawning handler task");
                    Console.Out.Flush();
                    
                    // Spawn handler on thread pool without waiting
                    _ = HandleClientAsync(client).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine($"[HandleClient] Task failed: {task.Exception?.InnerException?.Message}");
                            Console.Out.Flush();
                        }
                    }, TaskScheduler.Default);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"[Server Loop] Accept cancelled");
                    Console.Out.Flush();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Server Loop] Error during accept: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                    Console.Out.Flush();
                    // Don't throw, keep listening
                    await Task.Delay(100);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Fatal error: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            Console.Out.Flush();
            throw;
        }
        finally
        {
            Console.WriteLine($"[Server] Cleaning up listener");
            _listener.Stop();
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using (client)
            using (var stream = client.GetStream())
            {
                Console.WriteLine("[HandleClient] Starting");
                
                // Read request line
                var requestLineBytes = await ReadLineAsync(stream);
                var requestLine = Encoding.UTF8.GetString(requestLineBytes);
                Console.WriteLine($"[HandleClient] Got request line");

                if (string.IsNullOrWhiteSpace(requestLine))
                    return;

                var parts = requestLine.Split(' ');
                if (parts.Length < 2)
                    return;

                var method = parts[0];
                var path = parts[1];

                Console.WriteLine($"[HandleClient] {method} {path}");

                // Read headers
                var headers = new Dictionary<string, string>();
                while (true)
                {
                    var lineBytes = await ReadLineAsync(stream);
                    var line = Encoding.UTF8.GetString(lineBytes).TrimEnd('\r', '\n');
                    
                    if (string.IsNullOrEmpty(line))
                        break;
                    
                    var headerParts = line.Split(':', 2);
                    if (headerParts.Length == 2)
                    {
                        headers[headerParts[0].Trim()] = headerParts[1].Trim();
                    }
                }
                Console.WriteLine($"[HandleClient] Headers read");

                // Read body based on Content-Length
                byte[]? body = null;
                if (headers.TryGetValue("Content-Length", out var contentLengthStr) && int.TryParse(contentLengthStr, out var contentLength))
                {
                    if (contentLength > 0)
                    {
                        Console.WriteLine($"[HandleClient] Reading body: {contentLength} bytes");
                        Console.Out.Flush();
                        body = new byte[contentLength];
                        var totalRead = 0;
                        while (totalRead < contentLength)
                        {
                            var remaining = contentLength - totalRead;
                            Console.WriteLine($"[HandleClient] Reading {remaining} bytes (total so far: {totalRead})");
                            Console.Out.Flush();
                            var bytesRead = await stream.ReadAsync(body, totalRead, remaining);
                            if (bytesRead == 0)
                            {
                                Console.WriteLine($"[HandleClient] Stream closed before reading body");
                                Console.Out.Flush();
                                body = null;
                                break;
                            }
                            totalRead += bytesRead;
                            Console.WriteLine($"[HandleClient] Read {bytesRead} bytes, total: {totalRead}");
                            Console.Out.Flush();
                        }
                        if (body != null)
                        {
                            Console.WriteLine($"[HandleClient] Body read: {totalRead} bytes");
                            Console.Out.Flush();
                        }
                    }
                }

                var request = new SimpleHttpListenerRequest(method, path, headers, body);
                var response = new SimpleHttpListenerResponse(stream);

                Console.WriteLine($"[HandleClient] Processing request");
                Console.Out.Flush();
                var context = new SimpleHttpContext(request, response);
                await _router.HandleContextAsync(context);
                Console.WriteLine($"[HandleClient] Request completed");
                Console.Out.Flush();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HandleClient] Exception: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"[HandleClient] Stack: {ex.StackTrace}");
        }
    }

    private async Task<byte[]> ReadLineAsync(NetworkStream stream)
    {
        var buffer = new List<byte>();
        var singleByte = new byte[1];
        
        while (true)
        {
            var bytesRead = await stream.ReadAsync(singleByte, 0, 1);
            if (bytesRead == 0)
                break;
            
            buffer.Add(singleByte[0]);
            
            // Check for \n (end of line)
            if (singleByte[0] == '\n')
                break;
        }
        
        return buffer.ToArray();
    }
}
