using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusPassengerInfo : Controller
    {
        public IActionResult PassengerInfo(int selectedButtonsCount, List<string> selectedButtons, string plateNumber, string terminal, string distance, 
            string tariff, string level, string route, string operatorName, string scheduleDate, string scheduleTime, string destinationCity, string depatureCity, string arrivialDate, string departureDate)
        {
            ViewBag.PlateNumber = plateNumber;
            ViewBag.Terminal = terminal;
            ViewBag.Distance = distance;
            ViewBag.Tariff = tariff;
            ViewBag.Level = level;
            ViewBag.Route = route;
            ViewBag.OperatorName = operatorName;
            ViewBag.Date = scheduleDate;
            ViewBag.Time = scheduleTime;
            ViewBag.DestinationCity = destinationCity;
            ViewBag.DepatureCity = depatureCity; 
            ViewBag.SelectedButtonsCount = selectedButtonsCount;
            ViewBag.SelectedButtons = selectedButtons;
            ViewBag.ArrivalDate = arrivialDate;
            ViewBag.DepartureDate = departureDate;
            return View();
        }
    }
}
