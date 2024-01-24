using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;
using System.Globalization;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusConfirmationController : Controller
    {
        [HttpPost]
        public IActionResult ConfirmationPage(List<PassengerModel> passengers, string tariff, string depatureCity, string destinationCity, string terminal, string operatorName,
            string date, string plateNumber, DateTime arrivalDate, DateTime departureDate)
        {
            string dateFormat = "ddd, MMM d, yyyy";
            DateTime dateTime = DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture);
            string formattedDate = dateTime.ToString("yyyy-MM-dd");
            string[] parts = tariff.Split(' ');
            decimal tarrifDecimal = decimal.Parse(parts[0]);
            var schedule = new VwRouteSchedule()
            {
                DestCityName = destinationCity,
                Tariff = tarrifDecimal,
                DepatureCity = depatureCity,
                DepartureDate = departureDate,
                OperatorName = operatorName,
                Date = formattedDate,
                Terminal = terminal,
                PlateNumber = plateNumber,
                ArrivalDate = arrivalDate,
            };
            var wrap = new Wrap()
            {
                VwRouteScheduleData = schedule,
                PassengerModelData = passengers,
            }; 
            return View(wrap);
        }
    }
}
