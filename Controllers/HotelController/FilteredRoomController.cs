using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Controllers.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.HotelController
{
    public class FilteredRoomController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HotelListBuffer _hotelListBuffer;
        private readonly AuthenticationManager _authenticationManager;

        public FilteredRoomController(
            IHttpClientFactory httpClientFactory,
            AuthenticationManager authenticationManager,
            HotelListBuffer hotelListBuffer)
        {
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
            _hotelListBuffer = hotelListBuffer;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {

            var identificationResult = await _authenticationManager.identificationValid();
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
            var loginInfo = HttpContext.Session.GetString("IsLogin");
            var login = "";
            if (loginInfo != null)
            {
                login = JsonConvert.DeserializeObject<string>(loginInfo);
            }
            HttpContext.Session.Remove("IsLogin");
            if (login != "Yes")
            {
                if (!identificationResult.isValid && !identificationResult.isLoggedIn)
                {
                    string validation = "Hotel";
                    var validationJson = JsonConvert.SerializeObject(validation);
                    HttpContext.Session.SetString("SignInInformation", validationJson);
                    TempData["ErrorMessage"] = "Please login to proceed further.";
                    return RedirectToAction("Index", "Signin");
                }
            }

            var viewModelJson = HttpContext.Session.GetString("AvailabilityViewModel");
            var viewModel = new AvailabilityViewModel();
            if (!string.IsNullOrEmpty(viewModelJson))
            {
                HttpContext.Session.Remove("AvailabilityViewModel");
                viewModel = JsonConvert.DeserializeObject<AvailabilityViewModel>(viewModelJson);
            }
            return View(viewModel);
        }

    }
    
}
