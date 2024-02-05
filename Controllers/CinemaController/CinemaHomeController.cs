using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.CInemaModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace HulubejeBooking.Controllers.CinemaController
{
    public class CinemaHomeController : Controller
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly string _tmdbApiKey = "1ba83335ce22421020a77845254a578e";
        private readonly IHttpClientFactory _httpClientFactory;
        public CinemaHomeController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<MovieModel>> GetMoviesWithPosterUrls(List<MovieModel> movies)
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                ViewBag.UserName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Image = user?.personalattachment;
                ViewBag.SuccessCode = user?.successCode;
                ViewBag.Inumber = user?.idnumber;
                ViewBag.Idtype = user?.idtype;
                ViewBag.Dob = user?.dob;
                ViewBag.Idattachment = user?.idattachment;
                ViewBag.PhoneNumber = user?.phoneNumber;
                ViewBag.EmailAddress = user?.emailAddress;
            }
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

                        // Assign values to MovieModel properties
                        movie.PosterUrl = posterUrl;
                        movie.Overview = result.overview;
                        movie.GenreId = result.genre_ids;
                        movie.MovieId = result.id;
                        movie.BackdropPath = backdroppath;
                        moviesWithPosterUrls.Add(movie);
                    }
                }
            }
            return moviesWithPosterUrls;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var _client = _httpClientFactory.CreateClient("CnetHulubeje");
                DateTime currentDate = DateTime.Now;
                string formattedDate = currentDate.ToString("yyyy-MM-dd");

                HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"/Cinema/GetProductsForFilterAndPreview?industryType=LKUP000120765&date={formattedDate}");

                if (response.IsSuccessStatusCode )
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var movies = JsonConvert.DeserializeObject<List<MovieModel>>(responseData);

                    if (movies != null)
                    {
                        var moviesWithPosters = await GetMoviesWithPosterUrls(movies);
                        return View(moviesWithPosters);
                    }
                    else
                    {
                        return View(null);
                    }
                }
                else
                {
                    return View(null);
                }
            }
            catch (HttpRequestException)
            {
                return View(null);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Index(DateTime selectedDate)
        {
            try
            {
                var _client = _httpClientFactory.CreateClient("CnetHulubeje");
                string formattedDate = selectedDate.ToString("yyyy-MM-dd");
                HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"/Cinema/GetProductsForFilterAndPreview?industryType=LKUP000120765&date={formattedDate}");

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var movies = JsonConvert.DeserializeObject<List<MovieModel>>(responseData);
                    if (movies != null)
                    {
                        var moviesWithPosters = await GetMoviesWithPosterUrls(movies);
                        return View(moviesWithPosters);
                    }
                    else
                    {
                        return View(null);
                    }

                }
                else
                {
                    return View(null);
                }
            }
            catch (HttpRequestException)
            {
                return View(null);
            }
        }
    }
}
