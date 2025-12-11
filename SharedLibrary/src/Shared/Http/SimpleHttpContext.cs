using System.Collections;
using System.Net.Sockets;

namespace Shared.Http;

public class SimpleHttpContext : HttpContext
{
    public HttpRequest Request { get; }
    public HttpResponse Response { get; }
    public Hashtable Properties { get; }

    public SimpleHttpContext(HttpRequest request, HttpResponse response)
    {
        Request = request;
        Response = response;
        Properties = new Hashtable();
    }
}
