using System.Net;
using System.Net.Sockets;

var listener = new TcpListener(IPAddress.Loopback, 5001);
listener.Start();
Console.WriteLine("Test server listening on 5001");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    Console.WriteLine("Got connection!");
    _ = Task.Run(async () =>
    {
        using (client)
        using (var stream = client.GetStream())
        using (var writer = new StreamWriter(stream))
        {
            await writer.WriteAsync("HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nTest OK\r\n");
            await writer.FlushAsync();
        }
    });
}
