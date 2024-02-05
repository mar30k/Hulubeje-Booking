using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.CInemaModels;
using System.Text;
using HulubejeBooking.Controllers;
using HulubejeBooking.Models.Authentication;
namespace CinemaSeatBooking.Controllers
{
    public class CinemaFoodAndBeverageController : Controller
    {
        private readonly HttpClient _httpClient;
        private IHttpContextAccessor? _httpContextAccessor;

        public CinemaFoodAndBeverageController(IHttpContextAccessor? httpContextAccessor)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api/")
            };
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public async Task<IActionResult> Products([FromForm] string movieScheduleCode, [FromForm] string companyTinNumber, [FromForm] string branchCode, [FromForm] string companyName, [FromForm] string movieName,
                    [FromForm] string hallName, [FromForm] string utcTime, [FromForm] string selectedDate, [FromForm] decimal price, [FromForm] string dimension, [FromForm] string spaceType, [FromForm] string articleCode, [FromForm] string numberOfElements)
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
            HttpResponseMessage response = await _httpClient.GetAsync($"Product/GetProducts?orgTin={companyTinNumber}&type=Restaurant&consignee=0912141914&platform=Web&longitude=0");

            var viewModel = new ProductsViewModel
            {
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
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                var productsData = JsonConvert.DeserializeObject<List<Product>>(responseData);

                viewModel.Products = productsData;
                return View(viewModel);
            }
            else
            {

                return View(viewModel);
            }
        }
        public async Task<IActionResult> CalculateBill(string movieName,string branchCode,string movieDimension, string date, string time, string company, string hallName,
            string moviePrice, string movieScheduleCode, string companyTin, string movieArticleCode, string numberOfSeats, string selectedItems)
        {
            try
            {
                List<SelectedItem> selectedItemsList = JsonConvert.DeserializeObject<List<SelectedItem>>(selectedItems)!;

                var lineItems = new List<object>();
                ProductsViewModel calculatedModel = new();
                if (selectedItemsList != null)
                {
                    foreach (var selectedItem in selectedItemsList)
                    {
                        selectedItem.lineItemNote = "";
                        selectedItem.specialFlag = "";
                        lineItems.Add(selectedItem);
                    }
                }
                lineItems.Add(new
                {
                    articleName = movieName,
                    articleCode = movieArticleCode,
                    price = moviePrice,
                    quantity = numberOfSeats,
                    lineItemNote = "",
                    specialFlag = "@CNET_MOVIE_PRODUCT"
                });

                var payload = new
                {
                    Consignee = "0912141914",
                    OrgTin = companyTin,
                    IndustryType = "LKUP000120765",
                    Schedule = movieScheduleCode,
                    LineItems = lineItems
                };

                string jsonBody = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("LineItemTaxCalculator/CalculateOtherFeesForCinema", content);
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
                calculatedModel.SelectedItems = selectedItemsList;
                return PartialView("_Bill", calculatedModel);
            }
            catch (Exception)
            {

                return PartialView("_Bill", null);
            }
        }
    }

}