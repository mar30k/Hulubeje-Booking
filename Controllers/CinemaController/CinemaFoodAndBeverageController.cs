using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.CInemaModels;
using System.Text;
namespace CinemaSeatBooking.Controllers
{
    public class CinemaFoodAndBeverageController : Controller
    {
        private readonly HttpClient _httpClient;
        public CinemaFoodAndBeverageController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api/")
            };
        }
        [HttpPost]
        public IActionResult Products([FromForm] string movieScheduleCode, [FromForm] string companyTinNumber, [FromForm] string companyName, [FromForm] string movieName,
                    [FromForm] string hallName, [FromForm] string utcTime, [FromForm] string selectedDate, [FromForm] decimal price, [FromForm] string dimension, [FromForm] string spaceType, [FromForm] string articleCode, [FromForm] int numberOfElements)
        {
            // Convert the selectedSeats string to a list or array as needed
            return RedirectToAction("ProductsView", new
            {
                companyTinNumber,
                movieScheduleCode,
                companyName,
                hallName,
                utcTime,
                selectedDate,
                movieName,
                price,
                dimension,
                spaceType,
                articleCode,
                numberOfElements,
        });

        }

        public async Task<IActionResult> ProductsView(string companyTinNumber, string movieScheduleCode, string companyName, string hallName,
            string utcTime, string selectedDate, string movieName, decimal price, string dimension, string spaceType, string articleCode, string numberOfElements)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Product/GetProducts?orgTin={companyTinNumber}&type=Restaurant&consignee=0912141914&platform=Web&longitude=0");

            var viewModel = new ProductsViewModel
            {
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

        public async Task<IActionResult> CalculateBill(string movieName, string moviePrice, string movieScheduleCode, string companyTin, string movieArticleCode, string numberOfSeats, string selectedItems)
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

                return PartialView("_Bill", calculatedModel);
            }
            catch (Exception)
            {

                return PartialView("_Bill", null);
            }
        }
    }

}