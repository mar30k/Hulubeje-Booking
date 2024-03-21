
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.CInemaModels;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using HulubejeBooking.Controllers.Authentication;
using System.Security.Policy;
namespace HulubejeBooking.Controllers
{
    public class CinemaMovieDetailsController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly string _tmdbApiKey = "1ba83335ce22421020a77845254a578e";
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public CinemaMovieDetailsController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<IActionResult> Index(int companyCode, DateTime selectedDate, string movieCode, string companyName, string sanitizedOverview,
             string posterUrl, string movieName, int movieId, string streamUrl, string tin, int branchCode)
        {
            int? articleCode = 0;
            var movieJson = HttpContext.Session.GetString("movies");
            var movieData = movieJson != null ? JsonConvert.DeserializeObject<Movie>(movieJson) : new Movie();
            var movieSchedule = new List<MovieSchedules>();
            if (movieData != null && movieData.Data != null)
            {
                var company = movieData.Data.FirstOrDefault(c => c.CompanyCode == companyCode);

                if (company != null && company.Movies != null)
                {
                    var movie = company.Movies.FirstOrDefault(m => m.MovieName == movieName);
                    if (movie != null && movie.MovieSchedule != null )
                    {
                        movieSchedule = movie.MovieSchedule;
                    }
                    if (movie != null && movie.Date != null &&movie.Article!=null)
                    {
                        selectedDate = (DateTime)movie.Date;
                        articleCode = movie.Article;
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            var code = string.Empty;
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                code = user?.phoneNumber;
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
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
            var _tmdbClient = _httpClientFactory.CreateClient("MovieDb");
            var movieDetails = new MovieModel
            {
                MovieCode = movieCode,
                CompanyName = companyName,
                Overview = sanitizedOverview,
                PosterUrl = posterUrl,
                MovieName = movieName,
                SelectedDate = selectedDate,
                MovieSchedules = movieSchedule,
                CompanyCode = companyCode.ToString(),
                CompanyTinNumber = tin.ToString(),
                BranchCode = branchCode
            };
            var movieTitle = Uri.EscapeDataString(movieName ?? string.Empty);
            var tmdbClient = _httpClientFactory.CreateClient("MovieDb");
            var tmdbResponse = await tmdbClient.GetAsync(tmdbClient.BaseAddress + $"search/movie?api_key={_tmdbApiKey}&query={movieTitle}");
            int? tmdbMovieId = 0;
            if (tmdbResponse.IsSuccessStatusCode)
            {
                var tmdbData = await tmdbResponse.Content.ReadAsStringAsync();
                var movieDetail = JsonConvert.DeserializeObject<MovieDetails>(tmdbData);

                if (movieDetail != null && movieDetail?.results != null && movieDetail.results.Count > 0)
                {
                    var result = movieDetail.results[0];
                    var backdroppath = "https://image.tmdb.org/t/p/w500" + result.backdrop_path;
                    movieDetails.BackdropPath = backdroppath;
                    tmdbMovieId = result.id;
                }
            }
            string videoId = streamUrl?.Substring(streamUrl.IndexOf("v=") + 2) ?? "";
            movieDetails.YoutubeKey = videoId;
            string detailsApiUrl = $"movie/{tmdbMovieId}?api_key={_tmdbApiKey}&append_to_response=release_dates";
            HttpResponseMessage detailsResponse = await _tmdbClient.GetAsync(_tmdbClient.BaseAddress + detailsApiUrl);

            if (detailsResponse.IsSuccessStatusCode)
            {
                // Read and parse the movie details response content
                string detailsResponseData = await detailsResponse.Content.ReadAsStringAsync();
                        
                var movieDetail = JsonConvert.DeserializeObject<MovieModel>(detailsResponseData);
                if (movieDetail != null)
                {
                    movieDetails.RunTime = movieDetail.RunTime;
                    movieDetails.Genres = movieDetail.Genres;
                }
            }
            string castApiUrl = $"movie/{tmdbMovieId}/credits?api_key={_tmdbApiKey}";
            HttpResponseMessage castResponse = await _tmdbClient.GetAsync(_tmdbClient.BaseAddress + castApiUrl);

            if (castResponse.IsSuccessStatusCode)
            {
                string castResponseData = await castResponse.Content.ReadAsStringAsync();
                var castDetails = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(castResponseData);
                if (castDetails != null)
                {
                    var castList = ((IEnumerable<dynamic>)castDetails["cast"])
                    .Select(c => new CastMember
                    {
                        Name = c["name"].ToString(),
                        ProfilePath = "https://image.tmdb.org/t/p/w500" + c["profile_path"].ToString()
                    })
                    .ToList();
                    movieDetails.Cast = castList;                                
                }
            }
            movieDetails.MovieName = movieName;
            movieDetails.PhoneNumber = code;
            movieDetails.ArticleCode = articleCode.ToString();
            return View(movieDetails);   
        }
    }
}