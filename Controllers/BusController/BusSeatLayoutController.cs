using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusSeatLayoutController : Controller
    {
        public IActionResult SeatLayout()
        {
            return View();
        }
    }
}
