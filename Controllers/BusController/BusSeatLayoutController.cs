using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusSeatLayoutController : Controller
    {

        public IActionResult SeatLayout(string plateNumber, string terminal, string distance, string tariff, string level, string route, string operatorName, string scheduleDate, string scheduleTime, string destinationCity, string depatureCity)
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
            return View(schedueleInfo);
        }
    }
}
