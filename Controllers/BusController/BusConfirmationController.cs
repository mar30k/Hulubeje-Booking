using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusConfirmationController : Controller
    {
        [HttpPost]
        public IActionResult ConfirmationPage(List<PassengerModel> passengers, string tariff)
        {
            ViewBag.Tariff = tariff;
            Console.WriteLine(tariff);
            return View(passengers);
        }
    }
}
