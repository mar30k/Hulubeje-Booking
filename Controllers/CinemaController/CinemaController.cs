using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.CInemaModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using HulubejeBooking.Controllers.Authentication;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using NuGet.Common;
using HulubejeBooking.Models.BusModels;
namespace HulubejeBooking.Controllers.CinemaController
{
    public class CinemaController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        public CinemaController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<IActionResult> Index()
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var identificationResult = await _authenticationManager.identificationValid();

            string? token = identificationResult?.UserData?.Token;
            if (identificationResult != null)
            {
                ViewBag.isVaild = identificationResult.isValid;
                ViewBag.isLoggedIn = identificationResult.isLoggedIn;
                ViewBag.FirstName = identificationResult?.UserData.FirstName;
                ViewBag.LastName = identificationResult?.UserData.LastName;
                ViewBag.MiddleName = identificationResult?.UserData.MiddleName;
                ViewBag.Personalattachment = identificationResult?.UserData.PersonalAttachment;
                ViewBag.Idnumber = identificationResult?.UserData.IdNumber;
                ViewBag.Idtype = identificationResult?.UserData.IdType;
                ViewBag.Dob = identificationResult?.UserData.Dob;
                ViewBag.Idattachment = identificationResult?.UserData.IdAttachment;
                ViewBag.PhoneNumber = identificationResult?.UserData.Code;
                ViewBag.EmailAddress = identificationResult?.UserData.Email;
            }
            var movies = new Movie();
            DateTime dateTime = DateTime.Now;
            string formattedDate = dateTime.ToString("yyyy-MM-dd");
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage responseMessage =await _v7Client.GetAsync($"cinema/getconsolidatedmovies?Date={formattedDate}");
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseMessageData = await responseMessage.Content.ReadAsStringAsync();
                movies = responseMessageData!=null ?JsonConvert.DeserializeObject<Movie>(responseMessageData) : new Movie();
            }
            var moviesJson = JsonConvert.SerializeObject(movies);
            HttpContext.Session.SetString("movies", moviesJson);
            return View(movies);
        }
        [HttpPost]
        public async Task<IActionResult> Index(DateTime selectedDate)
        {
            var identificationResult = await _authenticationManager.identificationValid();
            string? token = identificationResult?.UserData?.Token;
            string? code = identificationResult?.UserData?.Code;
            try
            {
                var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
                var movies = new Movie();
                string formattedDate = selectedDate.ToString("yyyy-MM-dd");
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await _v7Client.GetAsync($"cinema/getconsolidatedmovies?Date={formattedDate}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseMessageData = await responseMessage.Content.ReadAsStringAsync();
                    movies = JsonConvert.DeserializeObject<Movie>(responseMessageData);
                }
                var moviesJson = JsonConvert.SerializeObject(movies);
                HttpContext.Session.SetString("movies", moviesJson);
                return View(movies);
            }
            catch (HttpRequestException)
            {
                return View(null);
            }
        }
    }
}
