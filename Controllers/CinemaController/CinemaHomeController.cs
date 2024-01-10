using HulubejeBooking.Models.CInemaModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.CinemaController
{
    public class CinemaHomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _tmdbApiKey = "1ba83335ce22421020a77845254a578e";
        private readonly string _tmdbApiBaseUrl = "https://api.themoviedb.org/3";
        public CinemaHomeController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api/")
            };

            _httpClient.DefaultRequestHeaders.Add("Api-Key", "YourApiKey");
        }
        public async Task<List<MovieModel>> GetMoviesWithPosterUrls(List<MovieModel> movies)
        {
            var moviesWithPosterUrls = new List<MovieModel>();
            foreach (var movie in movies)
            {
                var movieTitle = Uri.EscapeDataString(movie.MovieName ?? string.Empty);
                var apiUrl = $"{_tmdbApiBaseUrl}/search/movie?api_key={_tmdbApiKey}&query={movieTitle}";

                using (var tmdbClient = new HttpClient())
                {
                    var tmdbResponse = await tmdbClient.GetAsync(apiUrl);
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
            }
            return moviesWithPosterUrls;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                string formattedDate = currentDate.ToString("yyyy-MM-dd");

                HttpResponseMessage response = await _httpClient.GetAsync($"Cinema/GetProductsForFilterAndPreview?industryType=LKUP000120765&date={formattedDate}");

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    var movies = JsonConvert.DeserializeObject<List<MovieModel>>(responseData);

                    // Call the method to get movies with poster URLs
                    var moviesWithPosters = await GetMoviesWithPosterUrls(movies);

                    return View(moviesWithPosters);
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
                // Format the selected date
                string formattedDate = selectedDate.ToString("yyyy-MM-dd");

                // Make an HTTP GET request with the selected date
                HttpResponseMessage response = await _httpClient.GetAsync($"Cinema/GetProductsForFilterAndPreview?industryType=LKUP000120765&date={formattedDate}");

                if (response.IsSuccessStatusCode)
                {
                    // If the request is successful, read and deserialize the response
                    string responseData = await response.Content.ReadAsStringAsync();
                    var movies = JsonConvert.DeserializeObject<List<MovieModel>>(responseData);

                    // Call the method to get movies with poster URLs
                    var moviesWithPosters = await GetMoviesWithPosterUrls(movies);

                    // Return the view with the list of movies
                    return View(moviesWithPosters);
                }
                else
                {
                    // If the request is not successful, return an error view
                    return View(null);
                }
            }
            catch (HttpRequestException)
            {
                // If an exception occurs during the HTTP request, return an error view
                return View(null);
            }
        }
    }
}
