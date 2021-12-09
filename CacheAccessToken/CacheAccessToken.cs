using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace CacheAccessToken
{
    #region Interface

    public interface ITokenService
    {
        Task<TokenResult> GetToken();
    }

    #endregion

    #region TokenService

    public class TokenService : ITokenService
    {
        const string ACCESS_TOKEN = "AccessToken";
        private readonly ObjectCache Cache = MemoryCache.Default;

        public async Task<TokenResult> GetToken()
        {
            if (Cache.Contains(ACCESS_TOKEN))
            {
                return Cache.Get(ACCESS_TOKEN) as TokenResult;
            }

            return await GenerateAccessToken();
        }

        private async Task<TokenResult> GenerateAccessToken()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new ClientHelper().GetClient();
            HttpResponseMessage response = await client.PostAsync(GetTokenUrl(), null);

            string jsonResult = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // deserialise error
                var errorResult = JsonConvert.DeserializeObject<ErrorResult>(jsonResult);
                throw new Exception(errorResult.ErrorDescription);
            }

            //deseralise success
            var tokenResult = JsonConvert.DeserializeObject<TokenResult>(jsonResult);

            // Even if you were told that your session expired in two hours, it might not last two hours if an administrator revokes the session, the session remains in use, etc.
            // Sessions expire based on your organization's policy for sessions. Basically, as long as the app is in active use, the session won't expire.
            // Once the session is logged out, the timeout has elapsed,
            // or it is otherwise expired (e.g. an administrator expires all sessions for the Connected App).
            Cache.Add(ACCESS_TOKEN, tokenResult, DateTimeOffset.Now.AddMinutes(60.0));

            return tokenResult;
        }

        private string GetTokenUrl()
        {
            //var tokenUrl = "oauth2/token?grant_type{0}&amp;client_id{1}&amp;client_secret={2}&amp;username={3}&amp;password={4}";
            return string.Format(ConfigHelper.TokenUrl, ConfigHelper.GrantType, ConfigHelper.ClientId, ConfigHelper.ClientSecret, ConfigHelper.UserName, ConfigHelper.Password);
        }

        #region Parse Set-Cookie to get Max-Age
        private Dictionary<string, string> ParseCookies(HttpResponseMessage response)
        {
            var collection = new Dictionary<string, string>();
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookie))
            {
                foreach (var cookies in setCookie)
                {
                    string[] values = cookies.Split(';');
                    foreach (var cookie in values)
                    {
                        string[] a = cookie.Split('=');
                        if (a.Length != 2)
                        {
                            continue;
                        }

                        if (collection.ContainsKey(a[0]))
                        {
                            collection.Add(a[0], a[1]);
                        }
                    }
                }
            }
            return collection;
        }

        private Dictionary<string, string> ParseCookiesLinq(HttpResponseMessage response)
        {
            var collection = new Dictionary<string, string>();

            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookies))
            {
                foreach (string[] keyValue in from setCookie in setCookies
                                              let cookies = setCookie.Split(';')
                                              from a in
                                                  from cookie in cookies
                                                  let kv = cookie.Split('=')
                                                  select kv
                                              select a)
                {
                    if (keyValue.Length != 2)
                    {
                        continue;
                    }

                    if (collection.ContainsKey(keyValue[0]))
                    {
                        collection.Add(keyValue[0], keyValue[1]);
                    }
                }
            }
            return collection;
        }
        
        #endregion
    }

    #endregion

    #region ClientHelper

    internal class ClientHelper
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

    #endregion

    #region Models

    internal class ConfigHelper
    {
        public static string BaseUrl { get; internal set; } = ConfigurationManager.AppSettings["BaseUrl"];
        public static string TokenUrl { get; internal set; } = ConfigurationManager.AppSettings["TokenUrl"];
        public static string GrantType { get; internal set; } = ConfigurationManager.AppSettings["GrantType"];
        public static string ClientId { get; internal set; } = ConfigurationManager.AppSettings["ClientId"];
        public static string ClientSecret { get; internal set; } = ConfigurationManager.AppSettings["ClientSecret"];
        public static string UserName { get; internal set; } = ConfigurationManager.AppSettings["UserName"];
        public static string Password { get; internal set; } = ConfigurationManager.AppSettings["Password"];
    }

    internal class ProxyConfig
    {
        public static string Url { get; internal set; } = ConfigurationManager.AppSettings["ProxyAddress"];
        public static string UserName { get; internal set; } = ConfigurationManager.AppSettings["ProxyUserName"];
        public static string Password { get; internal set; } = ConfigurationManager.AppSettings["ProxyPassword"];
        public static string Domain { get; internal set; } = ConfigurationManager.AppSettings["ProxyDomain"];
    }

    public class ErrorResult
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }

    public class TokenResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("instance_url")]
        public string InstanceUrl { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("issued_at")]
        public string IssuedAt { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }

    #endregion
}