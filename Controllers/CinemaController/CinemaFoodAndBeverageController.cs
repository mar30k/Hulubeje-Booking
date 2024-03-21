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
        private IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;
        private IHttpClientFactory _httpClientFactory;
        public CinemaFoodAndBeverageController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync([FromForm] string movieScheduleCode, [FromForm] string companyTinNumber, [FromForm] string branchCode, [FromForm] string companyName, [FromForm] string movieName,
                    [FromForm] string hallName, [FromForm] string utcTime, [FromForm] string selectedDate, [FromForm] decimal price, [FromForm] string dimension, [FromForm] string spaceType,
                    [FromForm] string articleCode, [FromForm] string numberOfElements, [FromForm] string seatCacheKey, [FromForm] string companyCode)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");

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
            

            var viewModel = new ProductsViewModel
            {
                CompanyCode = companyCode,
                SeatCacheKey = seatCacheKey,
                BranchCode = branchCode,
                MovieScheduleCode = movieScheduleCode,
                CompanyTinNumber = companyTinNumber,
                CompanyName = companyName,
                HallName = hallName,
                ScheduleTime = utcTime,
                ScheduleDate = selectedDate,
                MovieName = movieName,
                Price = price,
                Dimension = dimension,
                SpaceType = spaceType,
                ArticleCode = articleCode,
                NumberOfSeats = numberOfElements,
            };
            
            if (HttpContext.Session.TryGetValue("loginToken", out var loginToken))
            {
                string token = Encoding.UTF8.GetString(loginToken);
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _v7Client.GetAsync($"product/getproductsbytype?companyCode={companyCode}&orgOUD={branchCode}&industryType=1988");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    var productsData = JsonConvert.DeserializeObject<FoodItem>(responseData);

                    viewModel.FoodItem = productsData;
                    return View(viewModel);
                }
                else
                {
                    return View(viewModel);
                }

            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> CalculateBill(string movieName,string branchCode,string movieDimension, string date, string time, string company, string hallName,
            decimal moviePrice, string movieScheduleCode, string companyTin, string movieArticleCode, string numberOfSeats, string selectedItems, string companyCode, string seatCacheKey)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");

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
                if (HttpContext.Session.TryGetValue("loginToken", out var loginToken)) {
                    string token = Encoding.UTF8.GetString(loginToken);
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
                calculatedModel.NumberOfSeats = numberOfSeats;
                calculatedModel.CompanyCode = companyCode;
                calculatedModel.SeatCacheKey = seatCacheKey;
                return PartialView("_Bill", calculatedModel);
            }
            catch (Exception)
            {

                return PartialView("_Bill", null);
            }
        }
    }

}