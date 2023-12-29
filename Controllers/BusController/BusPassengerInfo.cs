using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusPassengerInfo : Controller
    {
        public IActionResult PassengerInfo(int selectedButtonsCount, List<string> selectedButtons)
        {
            ViewBag.SelectedButtonsCount = selectedButtonsCount;
            ViewBag.SelectedButtons = selectedButtons;
            return View();
        }
    }
}
