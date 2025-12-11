using System.Net;
using System.Net.Sockets;

var ipAddress = IPAddress.Loopback;
Console.WriteLine($"Creating listener on {ipAddress}:5000");
var listener = new TcpListener(ipAddress, 5000);
Console.WriteLine("Starting listener");
listener.Start();
Console.WriteLine("Listener started, accepting...");

try
{
    var task = listener.AcceptTcpClientAsync();
    Console.WriteLine("AcceptTcpClientAsync() returned task");
    await Task.Delay(2000);
    Console.WriteLine($"After delay, task status: {task.Status}");
}
finally
{
    listener.Stop();
}
Console.WriteLine("Done");
