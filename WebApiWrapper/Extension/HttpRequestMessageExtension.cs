using System.Net.Http;
using WebApiWrapper.Model;
using WebApiWrapper.Service;

namespace WebApiWrapper.Extension
{
    public static class HttpRequestMessageExtension
    {
        public static void AddTokenAuthorization(this HttpRequestMessage request)
        {
            ITokenService tokenService = new TokenService();
            TokenResult tokenResult = System.Threading.Tasks.Task.Run(() => tokenService.GetToken()).Result;
            if (tokenResult != null)
            {
                request.Headers.Add("Authorization",string.Format("{0} {1}", tokenResult.TokenType, tokenResult.AccessToken));
            }
        }
    }
}