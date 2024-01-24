using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.Authentication
{
    public class Signup : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
