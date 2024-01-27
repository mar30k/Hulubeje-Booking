using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.Authentication
{
    public class SIgnInController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
