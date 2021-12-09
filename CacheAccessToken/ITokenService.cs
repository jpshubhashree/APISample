using CacheAccessToken.Model;
using System.Threading.Tasks;

namespace CacheAccessToken
{
    public interface ITokenService
    {
        Task<TokenResult> GetToken();
    }

}