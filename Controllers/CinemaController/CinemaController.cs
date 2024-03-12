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
        private readonly string _tmdbApiKey = "1ba83335ce22421020a77845254a578e";
        private readonly IHttpClientFactory _httpClientFactory;
        public CinemaController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<List<MovieModel>> GetMoviesWithPosterUrls(List<MovieModel> movies)
        {
            var moviesWithPosterUrls = new List<MovieModel>();
            foreach (var movie in movies)
            {
                var movieTitle = Uri.EscapeDataString(movie.MovieName ?? string.Empty);
                var tmdbClient = _httpClientFactory.CreateClient("MovieDb");
                var tmdbResponse = await tmdbClient.GetAsync(tmdbClient.BaseAddress + $"search/movie?api_key={_tmdbApiKey}&query={movieTitle}");
                if (tmdbResponse.IsSuccessStatusCode)
                {
                    var tmdbData = await tmdbResponse.Content.ReadAsStringAsync();
                    var movieDetails = JsonConvert.DeserializeObject<MovieDetails>(tmdbData);

                    if (movieDetails != null && movieDetails.results != null && movieDetails.results.Count > 0)
                    {
                        var result = movieDetails.results[0];
                        var posterUrl = "https://image.tmdb.org/t/p/w500" + result.poster_path;
                        var backdroppath = "https://image.tmdb.org/t/p/w500" + result.backdrop_path;

                        movie.PosterUrl = posterUrl;
                        movie.Overview = result.overview;
                        movie.GenreId = result.genre_ids;
                        movie.MovieId = result.id;
                        movie.BackdropPath = backdroppath;   
                    }
                    moviesWithPosterUrls.Add(movie);
                }
            }
            return moviesWithPosterUrls;
        }
        public async Task<IActionResult> Index()
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var token = "";
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            string? password = "";
            string? code = "";
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                password = user != null ? user?.password : "";
                code = user != null ? user?.phoneNumber : "";       
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
            if (identificationResult.isValid || identificationResult.isLoggedIn)
            {
                var param = new
                {
                    code,
                    password,
                    isChangePassword = false
                };
                var jsonRequest = JsonConvert.SerializeObject(param);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage loginResponse = await _v7Client.PostAsync("auth/login", content);
                if (loginResponse.IsSuccessStatusCode)
                {
                    string loginData = await loginResponse.Content.ReadAsStringAsync();
                    var loginresponseData = JsonConvert.DeserializeObject<LoginAuthentication>(loginData);
                    if (loginresponseData != null && loginresponseData.IsSuccessful == true)
                    {
                        token = loginresponseData?.Data?.Token;
                        if (token != null) { HttpContext.Session.SetString("loginToken", token); }
                    }
                }
            }
            else
            {
                var param = new
                {
                    code = "0000000000",
                    password = "0000000000",
                    isChangePassword = false
                };
                var jsonRequest = JsonConvert.SerializeObject(param);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage loginResponse = await _v7Client.PostAsync("auth/login", content);
                if (loginResponse.IsSuccessStatusCode)
                {
                    var loginData = await loginResponse.Content.ReadAsStringAsync();
                    var loginresponseData = JsonConvert.DeserializeObject<LoginAuthentication>(loginData);
                    if (loginresponseData != null && loginresponseData.IsSuccessful == true)
                    {
                        token = loginresponseData?.Data?.Token;
                        if (token != null) { HttpContext.Session.SetString("loginToken", token); }
                    }
                }
            }
            var movies = new Movie();
            DateTime dateTime = DateTime.Now;
            string formattedDate = dateTime.ToString("yyyy-MM-dd");
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage responseMessage =await _v7Client.GetAsync($"cinema/getconsolidatedmovies?Date={formattedDate}");
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseMessageData = await responseMessage.Content.ReadAsStringAsync();
                movies = JsonConvert.DeserializeObject<Movie>(responseMessageData);
            }
            var moviesJson = JsonConvert.SerializeObject(movies);
            HttpContext.Session.SetString("movies", moviesJson);
            return View(movies);
        }
        [HttpPost]
        public async Task<IActionResult> Index(DateTime selectedDate)
        {
            try
            {
                var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
                var movies = new Movie();
                string formattedDate = selectedDate.ToString("yyyy-MM-dd");
                if (HttpContext.Session.TryGetValue("loginToken", out var loginToken))
                {
                    string token = Encoding.UTF8.GetString(loginToken);
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
                else
                {
                    TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                    return RedirectToAction("Index", "Home");
                }


            }
            catch (HttpRequestException)
            {
                return View(null);
            }
        }
    }
}
