using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.CInemaModels;
using System.Text;
using HulubejeBooking.Controllers;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Controllers.Authentication;
using System.Net.Http;
using HulubejeBooking.Models.BusModels;
using System.Net.Http.Headers;
namespace CinemaSeatBooking.Controllers
{
    public class CinemaFoodAndBeverageController : Controller
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        public CinemaFoodAndBeverageController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        [Route("moviemenu")]
        public async Task<IActionResult> IndexAsync(ProductsViewModel productsViewModel)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");


            var identificationResult = await _authenticationManager.identificationValid();

            if (identificationResult != null)
            {
                ViewBag.isVaild = identificationResult.isValid;
                ViewBag.isLoggedIn = identificationResult.isLoggedIn;
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

            var movieData = GetMovieDataFromSession();
            if (movieData != null && movieData.Data != null)
            {

                var company = movieData.Data.FirstOrDefault(c => c.CompanyCode == productsViewModel?.CompanyCode);
                productsViewModel.BranchCode = company?.BranchCode ?? 0;
                productsViewModel.CompanyName = company?.CompanyName ?? productsViewModel.CompanyName;
                productsViewModel.CompanyTinNumber = company?.TIN ?? productsViewModel.CompanyTinNumber;
                productsViewModel.BranchName = company?.BranchName ?? productsViewModel.BranchName;
                if (company != null && company.Movies != null)
                {
                    var movie = company.Movies.FirstOrDefault(m => m?.MovieName == productsViewModel.MovieName);

                    if (movie != null)
                    {
                        productsViewModel.MovieName  = movie?.MovieName ?? productsViewModel.MovieName;
                        productsViewModel.ScheduleDate = movie?.Date != null ? (DateTime)movie.Date : productsViewModel.ScheduleDate;
                        productsViewModel.ArticleCode = movie?.Article ?? productsViewModel.ArticleCode;
                        var movieSchedule = movie?.MovieSchedule?.Where(c => c.SchdetailId == productsViewModel.MovieScheduleCode).FirstOrDefault();
                        productsViewModel.HallName = movieSchedule?.MovieSpaces?.Where(c => c.SpaceId == productsViewModel.SpaceID).FirstOrDefault()?.CinemaHall ?? productsViewModel.HallName;
                    }

                }
            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }

            string? token = identificationResult?.UserData?.Token;

            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _v7Client.GetAsync($"product/getproductsbytype?companyCode={productsViewModel.CompanyCode}&" +
                $"orgOUD={productsViewModel.BranchCode}&industryType=1988");
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                var productsData = JsonConvert.DeserializeObject<FoodItem>(responseData);

                productsViewModel.FoodItem = productsData;
                return View(productsViewModel);
            }
            else
            {
                return View(productsViewModel);
            }
        }

        [Route("calculatebill")]
        public async Task<IActionResult> CalculateBill(string movieName,int branchCode,string movieDimension, DateTime date, string time, string company, string hallName,
            decimal moviePrice, int movieScheduleCode, string companyTin, int movieArticleCode, string numberOfSeats, string selectedItems, int companyCode, string seatCacheKey)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");

            
            var identificationResult = await _authenticationManager.identificationValid();
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
            string? token = identificationResult?.UserData?.Token;
            try
            {
                List<SelectedItem> selectedItemsList = JsonConvert.DeserializeObject<List<SelectedItem>>(selectedItems)!;
                var lineItems = new List<object>();
                ProductsViewModel calculatedModel = new();
                if (selectedItemsList != null)
                {
                    foreach (var selectedItem in selectedItemsList)
                    {
                        selectedItem.uom = 0;
                        selectedItem.specialFlag = null;
                        lineItems.Add(selectedItem);
                    } 
                }
                lineItems.Add(new
                {
                    name = movieName,
                    article = movieArticleCode,
                    unitAmount = moviePrice,
                    quantity = numberOfSeats,
                    specialFlag = ""
                });

                var payload = new
                {
                    code = companyCode,
                    LineItems = lineItems
                };

                string jsonBody = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _v7Client.PostAsync("lineitem/calculate", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var calculatedBill = JsonConvert.DeserializeObject<Bill>(responseData);
                    calculatedModel.Bill = calculatedBill;
                }
                else
                {
                    return PartialView("_Bill", null);
                }
                calculatedModel.MovieScheduleCode = movieScheduleCode;
                calculatedModel.MovieName = movieName;
                calculatedModel.CompanyTinNumber = companyTin;
                calculatedModel.BranchCode = branchCode;
                calculatedModel.CompanyName = company;
                calculatedModel.ScheduleDate = date;
                calculatedModel.ScheduleTime = time;
                calculatedModel.Dimension = movieDimension;
                calculatedModel.HallName = hallName;
                calculatedModel.ArticleCode = movieArticleCode;
                calculatedModel.SelectedItems = selectedItemsList;
                calculatedModel.Price = moviePrice;
                calculatedModel.Seats = numberOfSeats;
                calculatedModel.CompanyCode = companyCode;
                calculatedModel.SeatCacheKey = seatCacheKey;
                return PartialView("_Bill", calculatedModel);
            }
            catch (Exception)
            {

                return PartialView("_Bill", null);
            }
        }
        private Movie GetMovieDataFromSession()
        {
            var movieJson = HttpContext.Session.GetString("movies");
            return movieJson != null ? JsonConvert.DeserializeObject<Movie>(movieJson) ?? new Movie() : new Movie();
        }
    }

}