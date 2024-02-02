using HulubejeBooking.Models;
using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace HulubejeBooking.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HistoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index(string phoneNumber)
        {
            return View();
        }
        public async Task<IActionResult> BusHistory(string phoneNumber)
        {
            var busClient = _httpClientFactory.CreateClient("BusBooking");
            HttpResponseMessage response = await busClient.GetAsync($"history/gethistorybyphoneNumber?PhoneNumber={phoneNumber}");
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var history = JsonConvert.DeserializeObject<List<HistoryModel>>(responseData);
                return View(history);
            }
            else
            {
                return View(null);
            }
        }
        public async Task<IActionResult> CinemaHotelHistory(string phoneNumber)
        {
            var hulubejeClient = _httpClientFactory.CreateClient("CnetHulubeje");
            HttpResponseMessage response = await hulubejeClient.GetAsync(hulubejeClient.BaseAddress + $"/order/GetOrdersByConsigneeCode?consigneeCode={phoneNumber}&page=1" );
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var history = JsonConvert.DeserializeObject<OrdersModel>(responseData);
                return View(history);
            }else
            {
                return View(null);
            }
        }
    }
}
