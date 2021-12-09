using CacheAccessToken.Model;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace CacheAccessToken
{
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
    }
}