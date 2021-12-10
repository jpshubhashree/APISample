using System;
using System.Net;
using System.Net.Http;
using WebApiWrapper.Model;

namespace WebApiWrapper.Service
{
    public class Servicebase
    {
        public const string CONTENT_TYPE = "application/json";
        private readonly int MAX_RETRY = 1;

        public WrappedResponse HandleResponse(HttpResponseMessage response, out bool retry, int retryCount = 0)
        {
            retry = false;
            if (response == null)
            {
                return new WrappedResponse()
                {
                    Succcess = false,
                    JsonResponseString = "[{\"message\":\"Response is null\"}]"
                };
            }

            var wrappedResponse = new WrappedResponse()
            {
                JsonResponseString = response.Content.ReadAsStringAsync().Result
            };

            if (response.StatusCode != HttpStatusCode.OK || !response.IsSuccessStatusCode)
            {
                if (retryCount > MAX_RETRY)
                {
                    wrappedResponse.Succcess = false;
                    return wrappedResponse;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    retry = true;
                }
                wrappedResponse.Succcess = false;
                return wrappedResponse;
            }

            wrappedResponse.Succcess = true;
            return wrappedResponse;
        }

        public WrappedResponse HandleException(Exception ex)
        {
            var message = string.Empty;

            if (ex.InnerException != null)
            {
                message = ex.InnerException.Message;
            }
            else
            {
                message = ex.Message;
            }
            return new WrappedResponse()
            {
                Succcess = false,
                JsonResponseString = message
            };
        }
    }
}