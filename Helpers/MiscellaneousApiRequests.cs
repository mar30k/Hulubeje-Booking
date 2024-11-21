using System.Net.Http.Headers;
using System.Net.Http;

namespace HulubejeBooking.Helpers
{
    public class MiscellaneousApiRequests
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MiscellaneousApiRequests(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string?> GetServerTime(string token)
        {
            try
            {
                var responseMessageData = "";
                var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await _v7Client.GetAsync("setting/getservertime");
                if (responseMessage.IsSuccessStatusCode)
                {
                    responseMessageData = await responseMessage.Content.ReadAsStringAsync();
                }
                return responseMessageData ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
}
