using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.HotelController
{
    public class FilteredRoomController : Controller
    {
        public class FilteredHotelsController : Controller
        {
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly HotelListBuffer _hotelListBuffer;

            public FilteredHotelsController(
                IHttpClientFactory httpClientFactory,
                HotelListBuffer hotelListBuffer)
            {
                _httpClientFactory = httpClientFactory;
                _hotelListBuffer = hotelListBuffer;
            }

            [HttpGet]
            public IActionResult Index()
            {
                var viewModelJson = HttpContext.Session.GetString("AvailabilityViewModel");

                if (!string.IsNullOrEmpty(viewModelJson))
                {
                    HttpContext.Session.Remove("AvailabilityViewModel");

                    var viewModel = JsonConvert.DeserializeObject<AvailabilityViewModel>(viewModelJson);

                    return View(viewModel);
                }

                return View(new AvailabilityViewModel());
            }


        }
    }
}
