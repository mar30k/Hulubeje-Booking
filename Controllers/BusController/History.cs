using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace HulubejeBooking.Controllers.BusController
{
    public class History : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public History(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> IndexAsync(string phoneNumber)
        {
            var busClient = _httpClientFactory.CreateClient("BusBooking");
            HttpResponseMessage response = await busClient.GetAsync($"history/gethistorybyphoneNumber?PhoneNumber={phoneNumber}");
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var companyData = JsonConvert.DeserializeObject<List<HistoryModel>>(responseData);
                return View(companyData);
            }
            else
            {
                return View(null);
            }
        }
    }
}
