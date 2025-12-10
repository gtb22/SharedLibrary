using System.Collections;
using System.Net.Sockets;

namespace Shared.Http;

public class SimpleHttpContext : IHttpContext
{
    public IHttpRequest Request { get; }
    public IHttpResponse Response { get; }
    public Hashtable Properties { get; }

    public SimpleHttpContext(IHttpRequest request, IHttpResponse response)
    {
        Request = request;
        Response = response;
        Properties = new Hashtable();
    }
}
