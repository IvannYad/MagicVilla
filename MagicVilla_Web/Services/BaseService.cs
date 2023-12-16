using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Text.Json.Serialization;
using static MagicVilla_Utility.SD;
using System.Net.Http.Headers;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responceModel { get ; set ; }
        public IHttpClientFactory httpClientFactory { get; set; }
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            this.responceModel = new APIResponse();
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClientFactory.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                if (apiRequest.Data is not null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data)
                        , Encoding.UTF8, "application/json");
                }

                message.Method = apiRequest.ApiType switch
                {
                    ApiType.POST => HttpMethod.Post,
                    ApiType.PUT => HttpMethod.Put,
                    ApiType.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get
                };

                HttpResponseMessage? apiResponse = null;
                if (!string.IsNullOrEmpty(apiRequest.Token))
                {
                    client.DefaultRequestHeaders
                        .Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
                }
                apiResponse = await client.SendAsync(message);

                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                try
                {
                    APIResponse apiResponseTest = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if (apiResponseTest is not null && (apiResponseTest.StatusCode == System.Net.HttpStatusCode.BadRequest
                        || apiResponseTest.StatusCode == System.Net.HttpStatusCode.NotFound))
                    {
                        apiResponseTest.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        apiResponseTest.IsSuccess = false;
                        var res = JsonConvert.SerializeObject(apiResponseTest);
                        var returnObj = JsonConvert.DeserializeObject<T>(res);
                        return returnObj;
                    }
                }
                catch (Exception e)
                {
                    var exceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return exceptionResponse;
                }

                var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
                return APIResponse;
            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { ex.Message },
                    IsSuccess = false,
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }
    }
}
