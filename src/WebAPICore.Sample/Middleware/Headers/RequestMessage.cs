using System.Collections.Generic;
using  WebAPICore.Sample.Middleware.Domains;

namespace  WebAPICore.Sample.Middleware.Headers
{
    public class RequestMessage : HttpMessage
    {
        public RequestMessage(AppInfo appInfo, MessageHeaders headers, Dictionary<string, object> request)
            : base(appInfo, headers, request)
        {
            appInfo.messageType = "request";
        }

    }
}
