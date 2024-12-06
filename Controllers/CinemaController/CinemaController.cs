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
using HulubejeBooking.Models.PaymentModels;
using Movie = HulubejeBooking.Models.CInemaModels.Movie;
using HulubejeBooking.Helpers;
namespace HulubejeBooking.Controllers.CinemaController
{
    public class CinemaController : Controller
    {
        public MiscellaneousApiRequests _miscellaneousApiRequests;
        private readonly AuthenticationManager _authenticationManager;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        public CinemaController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager, MiscellaneousApiRequests miscellaneousApiRequests)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _miscellaneousApiRequests = miscellaneousApiRequests;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var identificationResult = await _authenticationManager.identificationValid();

            string? token = identificationResult?.UserData?.Token;
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
            var movies = new Movie();
            List<CompanyData>? trendingMovies = await GetTrendingMovies(token, "service/cinema/getTrendingMovies");
            List<CompanyData>? cachedMovies = await GetTrendingMovies(token, "service/cinema/getConsolidatedMovies");
            movies.Data = cachedMovies != null && cachedMovies.Count > 0 ? cachedMovies : (await Getmovies(DateTime.Today.ToString("yyyy-MM-dd"), token ?? "", "1")) as List<CompanyData> ?? new List<CompanyData>();
            movies.TrendingMovies = trendingMovies;
            var moviesJson = JsonConvert.SerializeObject(movies);
            HttpContext.Session.SetString("movies", moviesJson);
            return View(movies);
        }
        [HttpPost]
        public async Task<IActionResult> Index(DateTime selectedDate)
        {
            var identificationResult = await _authenticationManager.identificationValid();
            string? token = identificationResult?.UserData?.Token;
            
            try
            {
                string formattedDate = selectedDate.ToString("yyyy-MM-dd");
                Movie? movies = (await Getmovies(formattedDate, token ?? "", "2")) as Movie ?? new Movie();
                string serverTimeString = (await _miscellaneousApiRequests.GetServerTime(token ?? "") ?? "").Trim('\"');
                DateTimeOffset? serverTime = DateTimeOffset.Parse(serverTimeString);
                DateTime date = serverTime.Value.DateTime;
                if (selectedDate.ToString("MM/dd/yyyy") == date.ToString("MM/dd/yyyy"))
                {
                    List<CompanyData>? trendingMovies = await GetTrendingMovies(token, "service/cinema/getTrendingMovies");
                    movies.TrendingMovies = trendingMovies?.ToList();
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
        public async Task<object?> Getmovies(string formattedDate, string token, string rt)
        {
            try
            {
                var movies = new Movie();
                var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await _v7Client.GetAsync($"cinema/getconsolidatedmovies?Date={formattedDate}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseMessageData = await responseMessage.Content.ReadAsStringAsync();
                    movies = JsonConvert.DeserializeObject<Movie>(responseMessageData);
                }

                var result = movies?.Data?.ToList();
                return rt == "1" ? result ?? new List<CompanyData>() : movies ?? new Movie();
            }
            catch
            {
                return new object();
            }

        }

        public async Task<List<CompanyData>?> GetTrendingMovies(string? token, string endpoint)
        {
            try 
            {
                var trendingMovies = new List<CompanyData>();
                var _v7Cache = _httpClientFactory.CreateClient("HulubejeCache");
                _v7Cache.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await _v7Cache.GetAsync(endpoint);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseMessageData = await responseMessage.Content.ReadAsStringAsync();
                    trendingMovies = responseMessageData != null ? JsonConvert.DeserializeObject<List<CompanyData>>(responseMessageData) : new List<CompanyData>();
                }
                return trendingMovies ?? new List<CompanyData>();
            }
            catch   
            {
                return new List<CompanyData>();
            }
        }
    }
}