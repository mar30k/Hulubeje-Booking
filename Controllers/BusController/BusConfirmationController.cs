using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusConfirmationController : Controller
    {
        [HttpPost]
        public IActionResult ConfirmationPage(List<PassengerModel> passengers, string tariff, string depatureCity, string destinationCity, string terminal, string operatorName,
            string date, string plateNumber, DateTime arrivalDate, DateTime departureDate)
        {
            ViewBag.DepatureCity = depatureCity; ViewBag.DestinationCity = destinationCity; ViewBag.Terminal = terminal; ViewBag.OperatorName = operatorName;
            ViewBag.Tariff = tariff; ViewBag.Date = date; ViewBag.PlateNumber = plateNumber; ViewBag.ArrivalDate = arrivalDate; ViewBag.DepartureDate = departureDate;
            return View(passengers);
        }
    }
}
