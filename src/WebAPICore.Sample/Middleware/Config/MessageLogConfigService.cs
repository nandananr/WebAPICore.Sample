using Microsoft.Extensions.Options;

namespace WebAPICore.Sample.Middleware.Config
{
    public class MessageLogConfigService : IMessageLogConfigService
    {
        private readonly MessageLogConfig _messageConfig;
        public MessageLogConfigService(IOptions<MessageLogConfig> messageConfig)
        {
            _messageConfig = messageConfig.Value;
        }

        public string GetApplicationId()
        {
            return _messageConfig.ApplicationId;
        }

        public string GetExcludeUrlPatterns()
        {
            return _messageConfig.ExcludeUrlPatterns;
        }

        public string GetIncludeUrlPatterns()
        {
            return _messageConfig.IncludeUrlPatterns;
        }

        public string GetLoggerEnable()
        {
            return _messageConfig.EnableLog;
        }

        public string GetLoggerName()
        {
            return _messageConfig.Name;
        }

        public string GetProfile()
        {
            return _messageConfig.Profiles;
        }

        public string GetSkipResponse()
        {
            return _messageConfig.SkipResponse;
        }
    }

    public interface IMessageLogConfigService
    {
        string GetLoggerName();
        string GetApplicationId();
        string GetProfile();
        string GetLoggerEnable();
        string GetSkipResponse();
        string GetIncludeUrlPatterns();
        string GetExcludeUrlPatterns();
    }
}
