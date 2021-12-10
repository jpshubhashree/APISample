using System.Threading.Tasks;
using WebApiWrapper.Model;

namespace WebApiWrapper.Service
{
    public interface ITokenService
    {
        Task<TokenResult> GetToken();
    }

}