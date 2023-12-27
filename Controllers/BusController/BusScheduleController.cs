using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using Newtonsoft.Json;
using HulubejeBooking.Models.BusModels;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusScheduleController : Controller
    {
        private readonly HttpClient _httpClient;
        public BusScheduleController()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://192.168.1.25:8092/api/")
            };

            _httpClient.DefaultRequestHeaders.Add("x-api-key", "9BE090F9-7F52-4297-93A1-32D03D361DE9");
        }
        public async  Task<IActionResult> BusScheduleview(string depature,string destination, DateTime travelDate)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"routeschedule/getschedulesbyroute?route={destination}&date={travelDate}");
            if(response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var scheduleData = JsonConvert.DeserializeObject<List<VwRouteSchedule>>(responseData);
                foreach (var schedule in scheduleData)
                {
                    schedule.DepatureCity = depature;
                }
                return View(scheduleData);
            }
            return View(null);
        }
    }
}