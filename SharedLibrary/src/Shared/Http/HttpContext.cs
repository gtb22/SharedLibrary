using System.Collections;

namespace Shared.Http;

public interface HttpContext
{
    HttpRequest Request { get; }
    HttpResponse Response { get; }
    Hashtable Properties { get; }
}
