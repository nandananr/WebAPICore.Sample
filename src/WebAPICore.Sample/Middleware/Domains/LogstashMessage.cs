using System.Collections.Generic;

namespace  WebAPICore.Sample.Middleware.Domains
{
    public class LogstashMessage
    {
        public string application_id { get; set; }
        public string logger_name { get; set; }
        public string level { get; set; }

        public string thread_name { get; set; }
        public string HOSTNAME { get; set; }
        public string host { get; set; }
        public string port { get; set; }
        public string sessionId { get; set; }

        public Dictionary<string, string> headers { get; set; }
        public Dictionary<string, object> payloadObj { get; set; }
        public Dictionary<string, string> application { get; set; }

        public string message { get; set; }
        public string stack_trace { get; set; }

    }
}
