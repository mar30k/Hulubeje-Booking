using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace HulubejeBooking.Controllers.BusController
{
    public class BusSeatLayoutController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BusSeatLayoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> SeatLayout(string plateNumber, string terminal, string distance, string tariff, string level, string route, string operatorName,
            string scheduleDate, string scheduleTime, string destinationCity, string depatureCity, string arrivalDate, string departureDate, string vehicleOperatorId)
        {
            var busSeatLayoutClient = _httpClientFactory.CreateClient("BusBooking");
            var schedueleInfo = new BusSeatLayout
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
                DepatureCity = depatureCity,
                ArrivialDate = arrivalDate,
                DepartureDate = departureDate,
                VehicleOperatorId = vehicleOperatorId,
            };
            HttpResponseMessage response = await busSeatLayoutClient.GetAsync($"vehicles/getvehicleseatlayout?id={6915}");
            if (response.IsSuccessStatusCode) { 
               string resopnseData = await response.Content.ReadAsStringAsync();
               var seatLayout = JsonConvert.DeserializeObject<SeatLayout>(resopnseData);
               schedueleInfo.SeatLayout = seatLayout;
            }
            
            return View(schedueleInfo);
        }
    }
}
