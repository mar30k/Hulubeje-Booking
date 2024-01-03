using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using Newtonsoft.Json;
using HulubejeBooking.Models.BusModels;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusScheduleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BusScheduleController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async  Task<IActionResult> BusScheduleview(string depature,string destination, DateTime travelDate)
        {
            var busSeatLayoutClient = _httpClientFactory.CreateClient("BusBooking");
            HttpResponseMessage response = await busSeatLayoutClient.GetAsync($"routeschedule/getschedulesbyroute?route={destination}&date={travelDate}");
            if(response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var scheduleData = JsonConvert.DeserializeObject<List<VwRouteSchedule>>(responseData);
                if(scheduleData != null )
                {
                    foreach (var schedule in scheduleData)
                    {
                        schedule.DepatureCity = depature;
                    }
                }
                return View(scheduleData);
            }
            return View(null);
        }
    }
}