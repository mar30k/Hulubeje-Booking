using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Controllers.Authentication;

namespace HulubejeBooking.Controllers.HotelController
{
    public class FilteredHotelsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HotelListBuffer _hotelListBuffer;
        private readonly AuthenticationManager _authenticationManager;

        public FilteredHotelsController(
            IHttpClientFactory httpClientFactory,
            HotelListBuffer hotelListBuffer, AuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
            _hotelListBuffer = hotelListBuffer;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var viewModelJson = HttpContext.Session.GetString("HotelViewModel");

            if (!string.IsNullOrEmpty(viewModelJson))
            {
                HttpContext.Session.Remove("HotelViewModel");

                var viewModel = JsonConvert.DeserializeObject<HotelViewModel>(viewModelJson);
                var identificationResult = await _authenticationManager.identificationValid();

                if (identificationResult.isValid)
                {
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }

            }

            return View(new HotelViewModel());
        }
    }

}
