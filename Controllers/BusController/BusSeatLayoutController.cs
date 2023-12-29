using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace HulubejeBooking.Controllers.BusController
{
    public class BusSeatLayoutController : Controller
    {
        private readonly HttpClient _httpClient;
        public BusSeatLayoutController()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://192.168.1.25:8092/api/")
            };

            _httpClient.DefaultRequestHeaders.Add("x-api-key", "9BE090F9-7F52-4297-93A1-32D03D361DE9");
        }
        public async Task<IActionResult> SeatLayout(string plateNumber, string terminal, string distance, string tariff, string level, string route, string operatorName,
            string scheduleDate, string scheduleTime, string destinationCity, string depatureCity)
        {
            var schedueleInfo = new SeatLayoutFormData
            {
                Distance = distance,
                Level = level,
                Route = route,
                PlateNumber = plateNumber,
                Terminal = terminal,
                Tariff = tariff,
                OperatorName = operatorName,
                Date = scheduleDate,
                Time = scheduleTime,
                DestinationCity = destinationCity,
                DepatureCity = depatureCity
            };
            HttpResponseMessage response = await _httpClient.GetAsync($"vehicles/getvehicleseatlayout?id={6915}");
            if (response.IsSuccessStatusCode) { 
               string resopnseData = await response.Content.ReadAsStringAsync();
               var seatLayout = JsonConvert.DeserializeObject<SeatLayout>(resopnseData);
               schedueleInfo.SeatLayout = seatLayout;
            }
            
            return View(schedueleInfo);
        }
    }
}
