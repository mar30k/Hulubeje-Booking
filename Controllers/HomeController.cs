using HulubejeBooking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HulubejeBooking.Controllers.Authentication;
using System.Net.Http;
using HulubejeBooking.Models.HotelModels;
using Newtonsoft.Json;
using HulubejeBooking.Models.Authentication;
namespace HulubejeBooking.Controllers
{
    public class HomeController : Controller
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<HomeController> _logger;
        private readonly AuthenticationManager _authenticationManager;
		public HomeController(ILogger<HomeController> logger, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
		{
            _httpContextAccessor = httpContextAccessor;
			_authenticationManager = authenticationManager;
			_logger = logger;
			_httpClientFactory = httpClientFactory;
		}


		public async Task<IActionResult> Index()
        {
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");

            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                ViewBag.FirstName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Personalattachment = user?.personalattachment;
                ViewBag.SuccessCode = user?.successCode;
                ViewBag.Idnumber = user?.idnumber;
                ViewBag.Idtype = user?.idtype;
                ViewBag.Dob = user?.dob;
                ViewBag.Idattachment = user?.idattachment;
                ViewBag.PhoneNumber = user?.phoneNumber;
                ViewBag.EmailAddress = user?.emailAddress;
            }

            var identificationResult = await _authenticationManager.identificationValid();
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
			var Picures = new Images();
			var bus = "wbus";
            var hotel = "whotel";
            var spa = "wspa";
            var events = "wevent";
            var cinema = "wmovie";

            HttpResponseMessage busResponse = await _client.GetAsync(_client.BaseAddress + $"/Ecommerce/GetBannerImages?directory={bus}");

            if (busResponse.IsSuccessStatusCode)
            {
                string busData = await busResponse.Content.ReadAsStringAsync();
                var busImages = JsonConvert.DeserializeObject<List<string>>(busData);
                Picures.Bus = busImages ?? new List<string>();
            }
            HttpResponseMessage cinemaResponse = await _client.GetAsync(_client.BaseAddress + $"/Ecommerce/GetBannerImages?directory={cinema}");

            if (cinemaResponse.IsSuccessStatusCode)
            {
                string cinemaData = await cinemaResponse.Content.ReadAsStringAsync();
                var cinemaImages = JsonConvert.DeserializeObject<List<string>>(cinemaData);
                Picures.Cinema = cinemaImages ?? new List<string>();
            }
            HttpResponseMessage spaResponse = await _client.GetAsync(_client.BaseAddress + $"/Ecommerce/GetBannerImages?directory={spa}");

            if (spaResponse.IsSuccessStatusCode)
            {
                string spaData = await spaResponse.Content.ReadAsStringAsync();
                var spaImages = JsonConvert.DeserializeObject<List<string>>(spaData);
                Picures.Spa = spaImages ?? new List<string>();
            }
            HttpResponseMessage eventResposne = await _client.GetAsync(_client.BaseAddress + $"/Ecommerce/GetBannerImages?directory={events}");

            if (eventResposne.IsSuccessStatusCode)
            {
                string eventData = await eventResposne.Content.ReadAsStringAsync();
                var eventImages = JsonConvert.DeserializeObject<List<string>>(eventData);
                Picures.Event = eventImages ?? new List<string>();
            }
            HttpResponseMessage hotelResponse = await _client.GetAsync(_client.BaseAddress + $"/Ecommerce/GetBannerImages?directory={hotel}");

            if (hotelResponse.IsSuccessStatusCode)
            {
                string hotelData = await hotelResponse.Content.ReadAsStringAsync();
                var hotelImages = JsonConvert.DeserializeObject<List<string>>(hotelData);
                Picures.Hotel = hotelImages ?? new List<string>();
            }
            return View(Picures);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
