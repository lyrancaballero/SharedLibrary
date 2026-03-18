namespace Shared.Http;

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using System.Xml.Linq;
using Shared.Config;
public static class HttpUtils
{
    // <-- Rest of the code below goes here.
    public static async Task StructuredLogging(HttpListenerRequest req,
HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        var requestId = props["req.id"]?.ToString() ??
        Guid.NewGuid().ToString("n").Substring(0, 12);
        var startUtc = DateTime.UtcNow;
        var method = req.HttpMethod ?? "UNKNOWN";
        var url = req.Url!.OriginalString ?? req.Url!.ToString();
        var remote = req.RemoteEndPoint.ToString() ?? "unknown";
        res.Headers["X-Request-Id"] = requestId;
        try
        {
            await next();
        }
        finally
        {
            var duration = (DateTime.UtcNow - startUtc).TotalNanoseconds;
            var record = new
            {
                timestamp = startUtc.ToString("o"),
                requestId,
                method,
                url,
                remote,
                statusCode = res.StatusCode,
                contentType = res.ContentType,
                contentLength = res.ContentLength64,
                duration
            };
            Console.WriteLine(JsonSerializer.Serialize(record,
            JsonSerializerOptions.Web));
        }
    }
}