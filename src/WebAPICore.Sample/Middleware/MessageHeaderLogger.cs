using log4net;
using  WebAPICore.Sample.Middleware.Config;
using System.IO;
using System.Reflection;
using  WebAPICore.Sample.Middleware.Headers;
using  WebAPICore.Sample.Middleware.Domains;
using  WebAPICore.Sample.Middleware;
using System.Xml;

namespace  WebAPICore.Sample.Middleware
{
    public class MessageHeaderLogger
    {
        private static IMessageLogConfigService _loggerConfig   ;
        private static  string _loggerName;
        private static  string _applicationId;
        private static  string _loggerEnable;
        private static  string _profile;
        private static  ILog _logger;

        static MessageHeaderLogger()
        {           
            //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            //XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            //_logger = logRepository.GetLogger(_loggerName);
           
        }

        private MessageHeaderLogger()
        {

        }

        public static class LoggerHolder
        {
            public static MessageHeaderLogger MhLogger = new MessageHeaderLogger();
        }
        public static MessageHeaderLogger GetInstance(IMessageLogConfigService messageLog)
        {
            _loggerConfig = messageLog;
            _loggerName = _loggerConfig.GetLoggerName();
            _applicationId = _loggerConfig.GetApplicationId();
            _loggerEnable = _loggerConfig.GetLoggerEnable();
            _profile = _loggerConfig.GetProfile();
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));

            var repository = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repository, log4netConfig["log4net"]);
            _logger = log4net.LogManager.GetLogger(repository.Name, _loggerName);
            return LoggerHolder.MhLogger;
        }

        public void Info(HttpMessage message)
        {


            if (IsLoggerEnable() && _logger.IsInfoEnabled)
            {
                message.appInfo = EnrichAppInfo(message.appInfo);
                string json = LogstashEncoder.Encoder(LoggerLevel.Info, message);
                _logger.Info(json);  //.Info(json);
            }
        }

        public void Debug(HttpMessage message)
        {
            if (IsLoggerEnable() && _logger.IsDebugEnabled)
            {
                message.appInfo = EnrichAppInfo(message.appInfo);
                string json = LogstashEncoder.Encoder(LoggerLevel.Info, message);
                _logger.Debug(json);  //_logger.Debug(json);

            }
        }

        public void Warn(HttpMessage message)
        {
            if (IsLoggerEnable() && _logger.IsWarnEnabled)
            {
                message.appInfo = EnrichAppInfo(message.appInfo);
                string json = LogstashEncoder.Encoder(LoggerLevel.Info, message);
                _logger.Warn(json);  // _logger.Warn(json);

            }
        }

        public void Error(HttpMessage message)
        {
            if (IsLoggerEnable() && _logger.IsErrorEnabled)
            {
                message.appInfo = EnrichAppInfo(message.appInfo);
                string json = LogstashEncoder.Encoder(LoggerLevel.Info, message);
                _logger.Error(json);  // _logger.Error(json);

            }
        }
        public void Fatal(HttpMessage message)
        {
            if (IsLoggerEnable() && _logger.IsFatalEnabled)
            {
                message.appInfo = EnrichAppInfo(message.appInfo);
                string json = LogstashEncoder.Encoder(LoggerLevel.Fatal, message);
                _logger.Fatal(json);  //  _logger.Fatal(json);
            }
        }

        public bool IsLoggerEnable()
        {
            if (_loggerEnable != null && _loggerEnable == "true")
            {
                return true;
            }
            return false;
        }

        private AppInfo EnrichAppInfo(AppInfo appInfo)
        {
            appInfo.id = _applicationId;
            appInfo.logger_name = _loggerName;
            appInfo.profile = _profile;
            return appInfo;
        }
    }
}