using log4net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPICore.Sample.Middleware.Domains;
using WebAPICore.Sample.Middleware.Headers;
using WebAPICore.Sample.Middleware.Utils;
using WebAPICore.Sample.Middleware.Config;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace WebAPICore.Sample.Middleware
{
    public class LogHeaderMiddleware
    {

        private readonly ILog _logger = LogManager.GetLogger(typeof(LogHeaderMiddleware));
        private readonly MessageHeaderLogger _mhLogger;
        private readonly string _skipResponse;
        private readonly string _includeUrlPatternsString;
        private readonly string _excludeUrlPatternsString;

        private static List<string> _includeUrlPatternsList;
        private static List<string> _excludeUrlPatternsList;

        private readonly RequestDelegate _next;

        public LogHeaderMiddleware(RequestDelegate next, IMessageLogConfigService messageLog)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            _next = next;
            _mhLogger = MessageHeaderLogger.GetInstance(messageLog);
            _skipResponse = messageLog.GetSkipResponse();
            _includeUrlPatternsString = messageLog.GetIncludeUrlPatterns();
            _excludeUrlPatternsString = messageLog.GetExcludeUrlPatterns();

            //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            //XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                bool isValidUrlPattern = IsValidUrlPattern(Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(context.Request));

                if (_mhLogger.IsLoggerEnable() && isValidUrlPattern)
                {
                    AppInfo appInfo = await GetAppInfo(context);
                    MessageHeaders headers = GetMessageHeaders(GetRequiredHeaders(context));
                    Dictionary<string, object> requestObj = GetRequest(context.Request);

                    RequestMessage requestMessage = new RequestMessage(appInfo, headers, requestObj);
                    _mhLogger.Info(requestMessage);

                    var originalBodyStream = context.Response.Body;

                    using (var responseBody = new MemoryStream())
                    {
                        context.Response.Body = responseBody;

                        await _next(context);
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        bool isResponseLogEnabled = _skipResponse != null && _skipResponse == "false";
                        if (_mhLogger.IsLoggerEnable() && isResponseLogEnabled && isValidUrlPattern)
                        {
                            AppInfo appInfo1 = await GetAppInfo(context);
                            headers = GetMessageHeaders(GetRequiredHeaders(context));
                            Dictionary<string, object> responseObj = new Dictionary<string, object>();
                            responseObj.Add("content", text);
                            ResponseMessage responseMessage = new ResponseMessage(appInfo1, headers, responseObj);
                            _mhLogger.Info(responseMessage);
                        }
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
            }
            catch (Exception e)
            {

            }
            // _logger.Info(" Message Logging filter initiated");
            //var cultureQuery = context.Request.Query["culture"];
            //if (!string.IsNullOrWhiteSpace(cultureQuery))
            //{
            //    var culture = new CultureInfo(cultureQuery);

            //    CultureInfo.CurrentCulture = culture;
            //    CultureInfo.CurrentUICulture = culture;

            //}
            // Call the next delegate/middleware in the pipeline
            //return this._next(context);


        }


        //void OnEndRequest(object sender, System.EventArgs e)
        //{
        //    bool isValidUrlPattern = IsValidUrlPattern(HttpContext.Current.Request.RawUrl);
        //    bool isResponseLogEnabled = skipResponse != null && skipResponse == "false";
        //    if (MhLogger.IsLoggerEnable() && isResponseLogEnabled && isValidUrlPattern)
        //    {
        //        AppInfo appInfo = GetAppInfo();
        //        MessageHeaders headers = GetMessageHeaders(HttpContext.Current.Response.Headers);
        //        Dictionary<string, object> responseObj = new Dictionary<string, object>();
        //        responseObj.Add("content", _watcher.ToString());
        //        ResponseMessage responseMessage = new ResponseMessage(appInfo, headers, responseObj);
        //        MhLogger.Info(responseMessage);
        //    }
        //}

        private bool IsValidUrlPattern(string url)
        {
            return !IsExcludeUrl(url) && IsIncludeUrl(url);
        }
        private bool IsExcludeUrl(string url)
        {
            if (_excludeUrlPatternsList == null)
            {
                _excludeUrlPatternsList = new List<string>();

                if (!string.IsNullOrEmpty(_excludeUrlPatternsString))
                {
                    _excludeUrlPatternsList = _excludeUrlPatternsString.Split(';').ToList();
                }
            }
            if (_excludeUrlPatternsList.Count == 0)
            {
                return false;
            }
            foreach (string pattern in _excludeUrlPatternsList)
            {
                if (StringUtilities.CompareStrings(pattern, url))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsIncludeUrl(string url)
        {
            if (_includeUrlPatternsList == null)
            {
                _includeUrlPatternsList = new List<string>();

                if (!string.IsNullOrEmpty(_includeUrlPatternsString))
                {
                    _includeUrlPatternsList = _includeUrlPatternsString.Split(';').ToList();
                }
            }

            if (_includeUrlPatternsList.Count == 0)
            {
                return true;
            }
            foreach (string pattern in _includeUrlPatternsList)
            {
                if (StringUtilities.CompareStrings(pattern, url))
                {
                    return true;
                }
            }
            return false;
        }


        private async Task<AppInfo> GetAppInfo(HttpContext context)
        {
            AppInfo appInfo = await MessageUtil.GetDefaultAppInfo();

            if (context.Session != null)// if (HttpContext.Current.Session != null)
            {
                appInfo.sessionId = context.Session.Id; //HttpContext.Current.Session.SessionID;
            }
            appInfo.port = context.Request.Host.Port.ToString(); //HttpContext.Current.Request.Url.Port.ToString();

            if (appInfo.host_name.Equals(MessageUtil.UNKNOWN))
            {
                appInfo.host_name = context.Request.Host.ToUriComponent(); //HttpContext.Current.Request.Url.Host;
            }

            return appInfo;
        }
        private MessageHeaders GetMessageHeaders(NameValueCollection headers)
        {
            return new MessageHeaders(headers);

        }
        private Dictionary<string, object> GetRequest(HttpRequest request)
        {
            Dictionary<string, object> requestMap = new Dictionary<string, object>();
            requestMap.Add("method", request.Method);
            requestMap.Add("content", (GetParams(request)));
            requestMap.Add("query", request.QueryString.ToString());
            requestMap.Add("remoteAddr", GetRemoteIP(request));
            requestMap.Add("url", Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(request));// requestMap.Add("url", request.RawUrl);
            return requestMap;
        }

        private bool IsJsonPost(HttpRequest request)
        {
            const string jsonContentType = "application/json";

            if (request.Method.ToUpper() != "POST")//if (request.RequestType.ToUpper() != "POST")
            {
                return false;
            }

            //if (request.ContentType == "" && request.AcceptTypes != null)
            //{
            //    return request.AcceptTypes.Any(mimeType => mimeType.Contains(jsonContentType));
            //}

            return request.ContentType.Contains(jsonContentType);
        }

        private string GetParams(HttpRequest request)
        {
            if (IsJsonPost(request))
            {
                return GetJsonPost(request);
            }
            return StringUtilities.ConvertDictionaryToString(GetPostParams(request));
        }

        private Dictionary<string, string> GetPostParams(HttpRequest request)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //foreach (var k in request.Form.AllKeys)
            // {
            //     dict.Add(k, request.Form[k]);
            // }

            //TODO testing while debugging 26 Jul 17
            if (request.HasFormContentType)
                dict = request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            return dict;
        }

        private string GetJsonPost(HttpRequest request)
        {
            //var bytes = new byte[request.InputStream.Length];
            //request.InputStream.Read(bytes, 0, bytes.Length);
            //request.InputStream.Position = 0;
            //return Encoding.ASCII.GetString(bytes);

            //TODO testing while debugging 26 Jul 17
            string bodyStr;
            using (StreamReader reader = new StreamReader(request.Body, Encoding.ASCII, true, 1024, true))
            {
                bodyStr = reader.ReadToEnd();
            }
            request.Body.Position = 0;
            return bodyStr;

        }
        private string GetRemoteIP(HttpRequest request)
        {
            //TODO In debug check the values in request.HttpContext.Features [IMP] 26 Jul 17
            string ipAddress = ""; // request.HttpContext.Features.//request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return request.HttpContext.Connection.RemoteIpAddress.ToString(); //request.ServerVariables["REMOTE_ADDR"];
        }

        private NameValueCollection GetRequiredHeaders(HttpContext context)
        {
            // Convert IHeaderDictionary to NameValueCollection
            NameValueCollection headerCollection = null;
            headerCollection = context.Request.Headers as NameValueCollection;
            if (headerCollection == null)
            {
                headerCollection = new NameValueCollection();
                foreach (var header in context.Request.Headers)
                {
                    headerCollection.Add(header.Key, header.Value);
                }
            }
            return headerCollection;
        }
    }
}
