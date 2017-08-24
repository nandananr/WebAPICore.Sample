using System.Collections.Generic;
using  WebAPICore.Sample.Middleware.Domains;

namespace  WebAPICore.Sample.Middleware.Headers
{
    public abstract class HttpMessage : TAMessage
    {
        public MessageHeaders headers { get; }

        public Dictionary<string, object> payloadObj { get; }

        protected HttpMessage(AppInfo appInfo, MessageHeaders headers, Dictionary<string, object> payloadObj)
        {
            this.appInfo = appInfo;
            this.headers = headers;
            this.payloadObj = payloadObj;
        }
    }
}
