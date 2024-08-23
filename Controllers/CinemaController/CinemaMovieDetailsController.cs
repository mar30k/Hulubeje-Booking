
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.CInemaModels;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using HulubejeBooking.Controllers.Authentication;
using System.Security.Policy;
using System.Text;
namespace HulubejeBooking.Controllers
{
    public class CinemaMovieDetailsController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly string _tmdbApiKey = "1ba83335ce22421020a77845254a578e";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public CinemaMovieDetailsController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        [Route("moviedetails")]
        public async Task<IActionResult> Index(int companyCode, DateTime selectedDate, string movieCode, string companyName, string sanitizedOverview,
             string posterUrl, string movieName, string streamUrl, string tin, int branchCode, string branchName, string phoneNumber)
        {
            DateTime? releaseDate = DateTime.MinValue;
            int? articleCode = 0;
            var movieData = GetMovieDataFromSession("movies");
            var movieSchedule = new List<MovieSchedules>();
            if (movieData != null && movieData.Data != null)
            {

                var company = movieData.Data.FirstOrDefault(c => c.CompanyCode == companyCode);
                branchCode = company?.BranchCode ?? 0;
                companyName = company?.CompanyName ?? companyName;
                tin = company?.TIN ?? tin;
                branchName = company?.BranchName ?? branchName;
                if (company != null && company.Movies != null)
                {
                    var movie = company.Movies.FirstOrDefault(m => m?.MovieName == movieName);

                    if (movie != null)
                    {
                        releaseDate = movie.ReleaseDate;
                        movieSchedule = movie?.MovieSchedule ?? movieSchedule;
                        sanitizedOverview = movie?.Plot ?? sanitizedOverview;
                        movieName = movie?.MovieName ?? movieName;
                        streamUrl = movie?.StreamUrl ?? streamUrl;
                        sanitizedOverview = movie?.Plot?.ToString() ?? sanitizedOverview;
                        selectedDate = movie?.Date != null ? (DateTime)movie.Date : selectedDate;
                        articleCode = movie?.Article ?? articleCode;
                        posterUrl = movie?.MoviePoster ?? posterUrl;
                    }

                }
            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }
            var code = string.Empty;
            var identificationResult = await _authenticationManager.identificationValid();

            if (identificationResult != null)
            {
                phoneNumber = identificationResult?.UserData?.Code ?? phoneNumber;
                ViewBag.isVaild = identificationResult?.isValid;
                ViewBag.isLoggedIn = identificationResult?.isLoggedIn;
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
            var _tmdbClient = _httpClientFactory.CreateClient("MovieDb");
            var movieDetails = new MovieModel
            {
                ReleaseDate = releaseDate,
                MovieCode = movieCode,
                CompanyName = companyName,
                Overview = sanitizedOverview,
                PosterUrl = posterUrl,
                MovieName = movieName,
                SelectedDate = selectedDate,
                MovieSchedules = movieSchedule,
                CompanyCode = companyCode.ToString(),
                CompanyTinNumber = tin?.ToString(),
                BranchCode = branchCode,
                BranchName = branchName,
                PhoneNumber = phoneNumber
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
                    var result = movieDetail.results.Find(x => x.overview == sanitizedOverview);
                    var backdroppath = "https://image.tmdb.org/t/p/w500" + result?.backdrop_path;
                    movieDetails.BackdropPath = backdroppath;
                    tmdbMovieId = result?.id;
                }
            }
            string videoId = streamUrl?[(streamUrl.IndexOf("v=") + 2)..] ?? "";
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
            var castLists = await GetCastMembers(castApiUrl);
            movieDetails.Cast = castLists;
            movieDetails.MovieName = movieName;
            movieDetails.PhoneNumber = code;
            movieDetails.ArticleCode = articleCode.ToString();
            movieDetails.PhoneNumber = phoneNumber.ToString();
            //_ = await UpdateMovieAnalyticsAsync(tin, movieName, posterUrl);
            return View(movieDetails);
        }
        private Movie GetMovieDataFromSession(string type)
        {
            var movieJson = HttpContext.Session.GetString(type);
            return movieJson != null ? JsonConvert.DeserializeObject<Movie>(movieJson) ?? new Movie() : new Movie();
        }
        private async Task<bool> UpdateMovieAnalyticsAsync(string? orgTin, string? movieName, string posterUrl)
        {
            var param = new
            {
                orgTin,
                movieName,
                posterUrl
            };  
            var jsonRequest = JsonConvert.SerializeObject(param);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var _redCacheClient = _httpClientFactory.CreateClient("HulubejeCache");
            try
            {
                HttpResponseMessage responseMessage = await _redCacheClient.PostAsync($"service/cinema/updateMovieAnayltics", content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseMessageData = await responseMessage.Content.ReadAsStringAsync();
                    if (bool.TryParse(responseMessageData, out bool result))
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return false;
        }

        private async Task<List<CastMember>> GetCastMembers(string castApiUrl)
        {
            var _tmdbClient = _httpClientFactory.CreateClient("MovieDb");

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
                    return castList;
                }
            }
            return new List<CastMember>();
        }


        [HttpPost]
        public async Task<IActionResult> DeleteEntry([FromBody] SafePushEntry pushEntry)
        {
            try
            {
                var _seatCacheClient = _httpClientFactory.CreateClient("HulubejeCache");
                var Url = _seatCacheClient.BaseAddress?.OriginalString.ToString() + "cache/deleteEntry";
                var data = new
                {
                    key = pushEntry.Key,
                };
                var paramJson = JsonConvert.SerializeObject(data);
                var content = new StringContent(paramJson, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(Url),
                    Content = content
                };

                HttpResponseMessage entryExtensionresponse = await _seatCacheClient.SendAsync(request);

                if (entryExtensionresponse.IsSuccessStatusCode)
                {
                    var responseJson = await entryExtensionresponse.Content.ReadAsStringAsync();
                    return Ok(responseJson);
                }
                else
                {
                    return BadRequest("Error fetching seat status from cache.");
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
            catch (TaskCanceledException e)
            {
                return StatusCode(408, "Request timed out." + e.Message + e.Source);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}