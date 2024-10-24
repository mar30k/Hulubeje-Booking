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
        private readonly WorkWebContext _workWebContext;
		public HomeController(ILogger<HomeController> logger, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, WorkWebContext workWebContext)
		{
            _httpContextAccessor = httpContextAccessor;
			_authenticationManager = authenticationManager;
			_logger = logger;
			_httpClientFactory = httpClientFactory;
            _workWebContext = workWebContext;
		}


		public async Task<IActionResult> Index()
        {
            var identificationResult = await _authenticationManager.identificationValid();
            //while (identificationResult==null || identificationResult?.UserData?.FirstName == null)
            //{
            //    identificationResult = await _authenticationManager.identificationValid();
            //}
            var Picures = new Images
            {
                CookieValidation = identificationResult
            };
            if (identificationResult != null)
            {
                ViewBag.FirstName = identificationResult?.UserData?.FirstName;
                ViewBag.LastName = identificationResult?.UserData?.LastName;
                ViewBag.MiddleName = identificationResult?.UserData?.MiddleName;
                ViewBag.Personalattachment = identificationResult?.UserData?.PersonalAttachment;
                ViewBag.Idnumber = identificationResult?.UserData?.IdNumber;
                ViewBag.Idtype = identificationResult?.UserData?.IdType;
                ViewBag.Dob = identificationResult?.UserData?.Dob;
                ViewBag.Idattachment = identificationResult?.UserData?.IdAttachment;
                ViewBag.PhoneNumber = identificationResult?.UserData?.Code;
                ViewBag.EmailAddress = identificationResult?.UserData?.Email;
            }


            //var busResponse = await PicturesResonseAsync("wbus");
            //var cinemaResponse = await PicturesResonseAsync("wmovie");
            //var spaResponse = await PicturesResonseAsync("wspa");
            //var eventResposne = await PicturesResonseAsync("wevent");
            //var hotelResponse = await PicturesResonseAsync("whotel");

            //Picures.Bus = busResponse ?? new List<string>();
            //Picures.Cinema = cinemaResponse ?? new List<string>();
            //Picures.Event = eventResposne ?? new List<string>();
            //Picures.Hotel = hotelResponse ?? new List<string>();
            //Picures.Spa = spaResponse ?? new List<string>();
            
            return View(Picures);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<List<string>> PicturesResonseAsync(string type) {
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");

            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"/Ecommerce/GetBannerImages?directory={type}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var images = JsonConvert.DeserializeObject<List<string>>(data);
                return images ?? new List<string>();
            }
            return new List<string>();
        }
    }
}
