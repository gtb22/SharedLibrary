using System.Collections;

namespace Shared.Http;

public interface IHttpContext
{
    IHttpRequest Request { get; }
    IHttpResponse Response { get; }
    Hashtable Properties { get; }
}
