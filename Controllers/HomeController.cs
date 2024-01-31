using HulubejeBooking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HulubejeBooking.Controllers.Authentication;
namespace HulubejeBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthenticationManager _authenticationManager;
        public HomeController(ILogger<HomeController> logger, AuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            var identificationResult = await _authenticationManager.identificationValid();
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
