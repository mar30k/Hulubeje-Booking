using HulubejeBooking.Models.CInemaModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace HulubejeBooking.Controllers
{
    public class CinemaMovieDetailsController : Controller
    {
        private readonly string apiKey = "1ba83335ce22421020a77845254a578e";
        private readonly string baseUrl = "https://api.themoviedb.org/3/movie/";

        [HttpPost]
        public IActionResult Details(string selectedDate, string movieCode, string companyName, string overview,string articleCode,
             string posterUrl, string movieName, int movieId, string backdropPath)
        {
            return RedirectToAction("DetailsViewPage", new
            {
                selectedDate,
                movieCode,
                companyName,
                overview,
                articleCode,
                posterUrl,
                movieName,
                movieId,
                backdropPath,
            });
        }

        public async Task<IActionResult> DetailsViewPage(string selectedDate, string movieCode, string companyName, string overview, string articleCode,
    string posterUrl, string movieName, int movieId, string backdropPath)
        {
            try
            {
                // Construct the API URL for videos
                string videosApiUrl = $"{baseUrl}{movieId}/videos?api_key={apiKey}";

                // Make the API call for videos
                using HttpClient client = new();
                HttpResponseMessage videosResponse = await client.GetAsync(videosApiUrl);

                if (videosResponse.IsSuccessStatusCode)
                {
                    // Read and parse the videos response content
                    string videosResponseData = await videosResponse.Content.ReadAsStringAsync();
                    var videosResponseObj = JsonConvert.DeserializeObject<dynamic>(videosResponseData);

                    // Extract the key of the first trailer
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
                    // Construct the API URL for movie details
                    string detailsApiUrl = $"{baseUrl}{movieId}?api_key={apiKey}&append_to_response=release_dates";

                    // Make the API call for movie details
                    HttpResponseMessage detailsResponse = await client.GetAsync(detailsApiUrl);

                    if (detailsResponse.IsSuccessStatusCode)
                    {
                        // Read and parse the movie details response content
                        string detailsResponseData = await detailsResponse.Content.ReadAsStringAsync();
                        var movieDetails = JsonConvert.DeserializeObject<MovieModel>(detailsResponseData);

                        // Set additional properties in your MovieModel
                        if (movieDetails != null)
                        {
                            movieDetails.MovieCode = movieCode;
                            movieDetails.CompanyName = companyName;
                            movieDetails.Overview = overview;
                            movieDetails.ArticleCode = articleCode;
                            movieDetails.PosterUrl = posterUrl;
                            movieDetails.MovieName = movieName;
                            movieDetails.BackdropPath = backdropPath;
                            movieDetails.YoutubeKey = trailerKey; // Add the YoutubeKey property
                            movieDetails.SelectedDate = selectedDate;
                            var genreNames = new List<string>();
                            if (movieDetails.Genre != null)
                            {
                                foreach (var genreInfo in movieDetails.Genre)
                                {
                                    if (genreInfo is not null)
                                    {
                                        genreNames.Add((string)genreInfo);
                                    }
                                }
                            }

                            // Assign the genre names to the GenreNames property in MovieModel
                            movieDetails.GenreNames = genreNames;

                            string castApiUrl = $"{baseUrl}{movieId}/credits?api_key={apiKey}";
                            HttpResponseMessage castResponse = await client.GetAsync(castApiUrl);

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

                                    // Assign the castList to the Cast property in MovieModel
                                    movieDetails.Cast = castList;
                                }
                            }
                        }


                        string productsApiUrl = $"https://api-hulubeje.cnetcommerce.com/api/Cinema/GetProductsForFilterAndPreview?industryType=LKUP000120765&date={selectedDate:yyyy-MM-dd}";
                        HttpResponseMessage productsResponse = await client.GetAsync(productsApiUrl);
                        if (productsResponse.IsSuccessStatusCode)
                        {
                            string productsResponseData = await productsResponse.Content.ReadAsStringAsync();
                            var movieList = JsonConvert.DeserializeObject<List<MovieModel>>(productsResponseData);
                            if (movieList != null)
                            {
                                var movieInfo = movieList.FirstOrDefault(m => m.MovieName == movieName && m.CompanyName == companyName);
                                if (movieInfo != null && movieDetails != null)
                                {
                                    movieDetails.CompanyTinNumber = movieInfo.CompanyTinNumber;
                                }


                                if (movieInfo != null)
                                {
                                    // Retrieve company tin number and branch code
                                    var retrievedCompanyTinNumber = movieInfo.CompanyTinNumber;
                                    var retrievedBranchCode = movieInfo.BranchCode;

                                    // Use the retrieved information for the schedules API call
                                    string schedulesApiUrl = $"https://api-hulubeje.cnetcommerce.com/api/cinema/cinemaSchedules?orgTin={retrievedCompanyTinNumber}&date={selectedDate:yyyy-MM-dd} 11:11:11.326116&branchCode={retrievedBranchCode}";


                                    HttpResponseMessage schedulesResponse = await client.GetAsync(schedulesApiUrl);

                                    if (schedulesResponse.IsSuccessStatusCode)
                                    {
                                        // Read and parse the cinema schedules response content
                                        string schedulesResponseData = await schedulesResponse.Content.ReadAsStringAsync();
                                        var schedulesResponseObj = JsonConvert.DeserializeObject<dynamic>(schedulesResponseData);
                                        // Find schedules for the selected movie using the movie name
                                        if (schedulesResponseObj is not null)
                                        {
                                            var selectedMovie = ((IEnumerable<dynamic>)schedulesResponseObj.movies)
                                                .FirstOrDefault(m => m.movieName == movieName); if (selectedMovie != null)
                                            {
                                                // Extract schedules from the response
                                                List<MovieSchedule> schedules = JsonConvert.DeserializeObject<List<MovieSchedule>>(selectedMovie.movieSchedules.ToString());
                                                var pgRating = selectedMovie.pgRating;

                                                // Add schedules to the MovieModel
                                                if (movieDetails is not null)
                                                {
                                                    CultureInfo cultureInfo = CultureInfo.InvariantCulture;
                                                    string format24Hours = "hh:mm:ss tt";

                                                    movieDetails.Schedules = await Task.Run(()=>schedules.OrderBy(e=> DateTime.ParseExact(e.StartTime.ToString(), format24Hours, cultureInfo)).ToList());
                                                    movieDetails.PgRating = pgRating;
                                                }
                                            }
                                            else
                                            {
                                                // Handle case where schedules for the selected movie are not found
                                                return View(null);
                                            }
                                        }



                                    }
                                }
                                else
                                {
                                    // Handle schedules API error
                                    return View(null);
                                }

                                // Return the MovieDetails view with the combined model
                                return View(movieDetails);
                            }
                            else
                            {
                                // Handle case where movie information is not found in the response
                                return View(null);
                            }

                        }
                        else
                        {
                            // Handle products API error
                            return View(null);
                        }
                    }
                    else
                    {
                        // Handle movie details API error
                        return View(null);
                    }
                }
                else
                {
                    // Handle videos API error
                    return View(null);
                }
            }
            catch (Exception)
            {
                // Handle general exception
                return View(null);
            }
        }
    }
}