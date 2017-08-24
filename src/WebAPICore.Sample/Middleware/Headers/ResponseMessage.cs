using System.Collections.Generic;
using  WebAPICore.Sample.Middleware.Domains;

namespace  WebAPICore.Sample.Middleware.Headers
{
    public class ResponseMessage : HttpMessage
    {
        public ResponseMessage(AppInfo appInfo, MessageHeaders headers, Dictionary<string, object> response) : base(appInfo, headers, response)
        {
            appInfo.messageType = "response";
        }
    }
}
