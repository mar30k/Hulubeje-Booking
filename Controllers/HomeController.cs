using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
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
            if (identificationResult != null)
            {
                ViewBag.isVaild = identificationResult.isValid;
                ViewBag.isLoggedIn = identificationResult.isLoggedIn;
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
            var picturesUrl = await PicturesResonseAsync(1, identificationResult?.UserData?.Token);


            return View(picturesUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<List<string>> PicturesResonseAsync(int type, string? token) {
            var _client = _httpClientFactory.CreateClient("HulubejeBooking");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"routing/getbannerimages?industryType={type}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var images = JsonConvert.DeserializeObject<HulubejeResponse<List<string>>>(data);
                return images?.Data ?? new List<string>();
            }
            return new List<string>();
        }
    }
}
