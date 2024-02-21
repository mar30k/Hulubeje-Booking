using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.CInemaModels;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using HulubejeBooking.Controllers.Authentication;
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
        public async Task<IActionResult> Index(string selectedDate, string movieCode, string companyName, string sanitizedOverview,
             string posterUrl, string movieName, int movieId, string backdropPath)
        {
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
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
            var _tmdbClient = _httpClientFactory.CreateClient("MovieDb");
            var movieDetails = new MovieModel
            {
                MovieCode = movieCode,
                CompanyName = companyName,
                Overview = sanitizedOverview,
                PosterUrl = posterUrl,
                MovieName = movieName,
                BackdropPath = backdropPath,
                SelectedDate = selectedDate
            };
            var videosApiUrl = $"movie/{movieId}/videos?api_key={_tmdbApiKey}";
            HttpResponseMessage videosResponse = await _tmdbClient.GetAsync(_tmdbClient.BaseAddress + videosApiUrl);
            if (videosResponse.IsSuccessStatusCode)
            {
                string videosResponseData = await videosResponse.Content.ReadAsStringAsync();
                var videosResponseObj = JsonConvert.DeserializeObject<dynamic>(videosResponseData);

                string? trailerKey = null;
                if (videosResponseObj != null)
                {
                    foreach (var result in videosResponseObj.results)
                    {
                        if (result.type == "Trailer" && result.site == "YouTube")
                        {
                            trailerKey = result.key.ToString();
                            break;
                        }
                    }

                }
                movieDetails.YoutubeKey = trailerKey;
            }
            string detailsApiUrl = $"movie/{movieId}?api_key={_tmdbApiKey}&append_to_response=release_dates";
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
            string castApiUrl = $"movie/{movieId}/credits?api_key={_tmdbApiKey}";
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
                    movieDetails.Cast = castList;                                }
            }
            var productsApiUrl = $"/Cinema/GetProductsForFilterAndPreview?industryType=LKUP000120765&date={selectedDate:yyyy-MM-dd}";
            HttpResponseMessage productsResponse = await _client.GetAsync(_client.BaseAddress + productsApiUrl);
            if (productsResponse.IsSuccessStatusCode)
            {
                string productsResponseData = await productsResponse.Content.ReadAsStringAsync();
                var movieList = JsonConvert.DeserializeObject<List<MovieModel>>(productsResponseData);
                if (movieList != null)
                {
                    var movieInfo = movieList.FirstOrDefault(m =>
                        string.Equals(m.MovieName, movieName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(m.CompanyName, companyName, StringComparison.OrdinalIgnoreCase));

                    if (movieInfo != null && movieDetails != null)
                    {
                        movieDetails.CompanyTinNumber = movieInfo?.CompanyTinNumber;
                        movieDetails.BranchCode = movieInfo?.BranchCode;
                        movieDetails.ArticleCode = movieInfo?.ArticleCode;
                    }
                }
            }

            var retrievedCompanyTinNumber = movieDetails?.CompanyTinNumber;
            var retrievedBranchCode = movieDetails?.BranchCode;

            string schedulesApiUrl = $"/cinema/cinemaSchedules?orgTin={retrievedCompanyTinNumber}&date={selectedDate:yyyy-MM-dd} 11:11:11.326116&branchCode={retrievedBranchCode}";


            HttpResponseMessage schedulesResponse = await _client.GetAsync(_client.BaseAddress + schedulesApiUrl);

            if (schedulesResponse.IsSuccessStatusCode)
            {
                // Read and parse the cinema schedules response content
                string schedulesResponseData = await schedulesResponse.Content.ReadAsStringAsync();
                var schedulesResponseObj = JsonConvert.DeserializeObject<dynamic>(schedulesResponseData);
                // Find schedules for the selected movie using the movie name
                if (schedulesResponseObj is not null)
                {
                    var selectedMovie = ((IEnumerable<dynamic>)schedulesResponseObj.movies)
                                        .FirstOrDefault(m => m.movieName != null && m.movieName.ToString().Equals(movieName, StringComparison.OrdinalIgnoreCase));
                    if (selectedMovie != null)
                    {
                        List<MovieSchedule> schedules = JsonConvert.DeserializeObject<List<MovieSchedule>>(selectedMovie.movieSchedules.ToString());
                        var pgRating = selectedMovie.pgRating;


                        CultureInfo cultureInfo = CultureInfo.InvariantCulture;
                        string format24Hours = "hh:mm:ss tt";
                        if (movieDetails != null)
                        {
                            movieDetails.Schedules = await Task.Run(() =>
                            {
                                var orderedSchedules = schedules
                                    .Where(e => e.StartTime != null)
                                    .OrderBy(e => DateTime.ParseExact(e.StartTime!.ToString(), format24Hours, cultureInfo))
                                    .ToList();

                                return orderedSchedules;
                            });

                            movieDetails.PgRating = pgRating;
                        }
                    }
                }
            }
            return View(movieDetails);   
        }
    }
}