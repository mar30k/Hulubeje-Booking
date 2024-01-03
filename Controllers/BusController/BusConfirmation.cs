using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusConfirmation : Controller
    {
        public IActionResult ConfirmationPage()
        {
            return View();
        }
    }
}
