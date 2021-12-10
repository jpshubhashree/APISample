using System.Configuration;

namespace WebApiWrapper.Helper
{
    public class ConfigHelper
    {
        public static string BaseUrl { get; internal set; } = ConfigurationManager.AppSettings["BaseUrl"];
        public static string TokenUrl { get; internal set; } = ConfigurationManager.AppSettings["TokenUrl"];
        public static string GrantType { get; internal set; } = ConfigurationManager.AppSettings["GrantType"];
        public static string ClientId { get; internal set; } = ConfigurationManager.AppSettings["ClientId"];
        public static string ClientSecret { get; internal set; } = ConfigurationManager.AppSettings["ClientSecret"];
        public static string UserName { get; internal set; } = ConfigurationManager.AppSettings["UserName"];
        public static string Password { get; internal set; } = ConfigurationManager.AppSettings["Password"];
    }
}