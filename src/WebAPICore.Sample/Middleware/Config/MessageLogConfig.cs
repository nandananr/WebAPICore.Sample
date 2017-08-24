using Newtonsoft.Json;

namespace WebAPICore.Sample.Middleware.Config
{
    public class MessageLogConfig
    {
        [JsonProperty("logstash.logger.name")]
        public string Name { get; set; }

        [JsonProperty("logstash.logger.applicationid")]
        public string ApplicationId { get; set; }

        [JsonProperty("spring.profiles.active")]
        public string Profiles { get; set; }

        [JsonProperty("logstash.logger.enableLog")]
        public string EnableLog { get; set; }

        [JsonProperty("logstash.logger.skipResponse")]
        public string SkipResponse { get; set; }

        [JsonProperty("logstash.logger.includeUrlPatterns")]
        public string IncludeUrlPatterns { get; set; }

        [JsonProperty("logstash.logger.excludeUrlPatterns")]
        public string ExcludeUrlPatterns { get; set; }
    }
}
