using HulubejeBooking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HulubejeBooking.Controllers.Authentication;
using System.Net.Http;
using HulubejeBooking.Models.HotelModels;
using Newtonsoft.Json;
namespace HulubejeBooking.Controllers
{
    public class HomeController : Controller
    {

		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<HomeController> _logger;
        private readonly AuthenticationManager _authenticationManager;
		public HomeController(ILogger<HomeController> logger, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
		{
			_authenticationManager = authenticationManager;
			_logger = logger;
			_httpClientFactory = httpClientFactory;
		}


		public async Task<IActionResult> Index()
        {
			var _client = _httpClientFactory.CreateClient("CnetHulubeje");


			var identificationResult = await _authenticationManager.identificationValid();
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
			var busPics = new List<Images>();
			var bus = "wbus";
			//var hotel = "wbus";
			//var spa = "wbus";
			//var events = "wbus";
			//var cinema = "wbus";

			HttpResponseMessage busResponse = await _client.GetAsync(_client.BaseAddress + $"/Industry/GetAllCompanies?industryType={bus}");
			if ( busResponse.IsSuccessStatusCode ) 
			{
				string busData = await busResponse.Content.ReadAsStringAsync();
				busPics = JsonConvert.DeserializeObject<List<Images>>(busData);

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
