using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using  WebAPICore.Sample.Middleware.Domains;
using  WebAPICore.Sample.Middleware.Headers;
using  WebAPICore.Sample.Middleware.Apps;

namespace  WebAPICore.Sample.Middleware
{
    public static class LogstashEncoder
    {
        public static string Encoder(LoggerLevel level, HttpMessage message)
        {
            LogstashMessage lm = new LogstashMessage();

            AppInfo appInfo = message.appInfo;
            lm.HOSTNAME = appInfo.host_name;
            lm.host = appInfo.host;
            lm.thread_name = appInfo.thread_name;
            lm.port = appInfo.port;
            lm.sessionId = appInfo.sessionId;
            lm.logger_name = appInfo.logger_name;
            lm.application_id = appInfo.id;

            lm.headers = message.headers.ToDictionary();
            lm.application = GetApplication(appInfo);
            //request or response obj
            lm.payloadObj = message.payloadObj;

            lm.level = GetLoggerLevel(level);


            string json = JsonConvert.SerializeObject(lm, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return ReplaceVariables(json, appInfo.messageType);
        }

        public static string Encoder(LoggerLevel level, AppMessage message)
        {
            LogstashMessage lm = new LogstashMessage();

            AppInfo appInfo = message.appInfo;
            lm.HOSTNAME = appInfo.host_name;
            lm.host = appInfo.host;
            lm.thread_name = appInfo.thread_name;
            lm.port = appInfo.port;
            lm.sessionId = appInfo.sessionId;
            lm.logger_name = appInfo.logger_name;
            lm.application_id = appInfo.id;

            lm.message = message.message;
            ExceptionMessage exceptionMessage = message.exception;
            if (exceptionMessage != null)
            {
                lm.message = exceptionMessage.message;
                lm.stack_trace = exceptionMessage.stack_trace;
            }

            lm.level = GetLoggerLevel(level);

            string json = JsonConvert.SerializeObject(lm, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return ReplaceVariables(json, appInfo.messageType);
        }

        private static Dictionary<string, string> GetApplication(AppInfo appInfo)
        {
            Dictionary<string, string> application = new Dictionary<string, string>();
            application.Add("application_id", appInfo.id);
            application.Add("profile", appInfo.profile);
            application.Add("message_type", appInfo.messageType);
            return application;
        }
        private static string ReplaceVariables(string inputString, string newValue)
        {
            inputString = inputString.Replace("payloadObj", newValue);
            inputString = inputString.Replace("application_id", "application-id");
            return inputString;
        }

        private static string GetLoggerLevel(LoggerLevel level)
        {
            switch (level)
            {
                case LoggerLevel.Info:
                    return "INFO";
                case LoggerLevel.Debug:
                    return "DEBUG";
                case LoggerLevel.Warn:
                    return "WARN";
                case LoggerLevel.Error:
                    return "ERROR";
                case LoggerLevel.Fatal:
                    return "FATAL";
                default:
                    return "INFO";
            }
        }
    }
}

