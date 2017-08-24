using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using  WebAPICore.Sample.Middleware.Domains;

namespace  WebAPICore.Sample.Middleware.Utils
{
    public class MessageUtil
    {
        public static string UNKNOWN = "unknown";
        public static async Task<AppInfo> GetDefaultAppInfo()
        {
            AppInfo appInfo = new AppInfo();
            appInfo.thread_name = Thread.CurrentThread.ManagedThreadId.ToString();
            try
            {
                appInfo.host_name = Dns.GetHostName();
                appInfo.host = await GetServerIp(appInfo.host_name);
            }
            catch (Exception)
            {
                appInfo.host_name = UNKNOWN;
            }
            return appInfo;
        }

        public static async Task<string> GetServerIp(string hostname)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = await Dns.GetHostEntryAsync(Dns.GetHostName());

            IPAddress serverIp = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            var serverIpaddress = serverIp != null ? serverIp.ToString() : "not Found";
            return serverIpaddress;
        }
    }
}
