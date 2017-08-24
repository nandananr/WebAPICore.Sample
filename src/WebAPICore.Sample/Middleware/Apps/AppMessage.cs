using  WebAPICore.Sample.Middleware.Domains;

namespace  WebAPICore.Sample.Middleware.Apps
{
    public class AppMessage : TAMessage
    {
        public string message { get; }
        public ExceptionMessage exception { get; }

        public AppMessage(AppInfo appInfo, ExceptionMessage exception)
        {
            appInfo.messageType = "applog";
            this.appInfo = appInfo;
            this.exception = exception;
        }

        public AppMessage(AppInfo appInfo, string message)
        {
            appInfo.messageType = "applog";
            this.appInfo = appInfo;
            this.message = message;
        }

    }
}

