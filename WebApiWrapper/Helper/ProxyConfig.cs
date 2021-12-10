using System.Configuration;

namespace WebApiWrapper.Helper
{
    internal class ProxyConfig
    {
        public static string Url { get; internal set; } = ConfigurationManager.AppSettings["ProxyAddress"];
        public static string UserName { get; internal set; } = ConfigurationManager.AppSettings["ProxyUserName"];
        public static string Password { get; internal set; } = ConfigurationManager.AppSettings["ProxyPassword"];
        public static string Domain { get; internal set; } = ConfigurationManager.AppSettings["ProxyDomain"];
    }
}