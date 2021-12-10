using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiWrapper.Extension;
using WebApiWrapper.Helper;
using WebApiWrapper.Model;

namespace WebApiWrapper.Service
{
    public class UpdateService : Servicebase
    {
        public async Task<WrappedResponse> Update(string id, UpdateRequest request)
        {
            int retryCount = 0;
            try
            {
                var url = $"{ConfigHelper.BaseUrl}{id}";
                using(HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), url))
                {
                    requestMessage.AddTokenAuthorization();
                    var jsonData = JsonConvert.SerializeObject(request);
                    requestMessage.Content = new StringContent(jsonData, Encoding.UTF8, CONTENT_TYPE);
                    var clientHelper = new ClientHelper();
                    var response = await new ClientHelper().GetClient().SendAsync(requestMessage);

                    var resp =  HandleResponse(response, out bool retry, retryCount);

                    if(retry)
                    {
                        retryCount++;
                        await Update(id, request);
                    }

                    return resp;

                }
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}