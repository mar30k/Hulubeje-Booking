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
                ViewBag.UserName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Image = user?.personalattachment;
                ViewBag.Email = user?.emailAddress;
            }

            var identificationResult = await _authenticationManager.identificationValid();
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
			var busPics = new Images();
			var bus = "whotel";
            //var hotel = "wbus";
            //var spa = "wbus";
            //var events = "wbus";
            //var cinema = "wbus";

            HttpResponseMessage busResponse = await _client.GetAsync(_client.BaseAddress + $"/Ecommerce/GetBannerImages?directory={bus}");

            if (busResponse.IsSuccessStatusCode)
            {
                string busData = await busResponse.Content.ReadAsStringAsync();
                var busImages = JsonConvert.DeserializeObject<List<string>>(busData);
                busPics.Bus = busImages ?? new List<string>();
            }


            return View(busPics);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
