using System;
using System.Net;
using System.Net.Http;

namespace WebApiWrapper.Helper
{
    public class ClientHelper
    {
        public HttpClient GetClient()
        {
            return new HttpClient(GetHttpClientHandlerWithProxy())
            {
                BaseAddress = new Uri(ConfigHelper.BaseUrl)
            };
        }

        private HttpClientHandler GetHttpClientHandlerWithProxy()
        {
            return new HttpClientHandler()
            {
                UseProxy = true,
                Proxy = new WebProxy(ProxyConfig.Url)
                {
                    Credentials = new NetworkCredential()
                    {
                        UserName = ProxyConfig.UserName,
                        Password = ProxyConfig.Password,
                        Domain = ProxyConfig.Domain
                    }
                }
            };
        }

    }
}